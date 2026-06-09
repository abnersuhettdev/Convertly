using Convertly.Application.Auth;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Convertly.Domain.Constants;
using Convertly.Domain.Entities;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Convertly.Infrastructure.Auth;

public sealed class AuthService(
    ConvertlyDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IRefreshTokenGenerator refreshTokenGenerator,
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider) : IAuthService
{
    private const int RefreshTokenDays = 7;

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = ValidateRegisterRequest(request);
        if (validationErrors.Count > 0)
        {
            return ApiResponse<AuthResponse>.Fail("Validation failed", validationErrors.ToArray());
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailExists = await dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return ApiResponse<AuthResponse>.Fail("Registration failed", "Email is already registered");
        }

        var freePlan = await dbContext.Plans.SingleOrDefaultAsync(plan => plan.Slug == PlanSlugs.Free, cancellationToken);
        if (freePlan is null)
        {
            return ApiResponse<AuthResponse>.Fail("Registration failed", "Free plan was not found");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = now,
            IsActive = true
        };

        var subscription = new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PlanId = freePlan.Id,
            Status = SubscriptionStatus.Active,
            StartedAt = now,
            CreatedAt = now
        };

        dbContext.Users.Add(user);
        dbContext.UserSubscriptions.Add(subscription);

        var response = CreateAuthResponse(user, now);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ApiResponse<AuthResponse>.Ok(response, "User registered successfully");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = ValidateLoginRequest(request);
        if (validationErrors.Count > 0)
        {
            return ApiResponse<AuthResponse>.Fail("Validation failed", validationErrors.ToArray());
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(
            user => user.Email == normalizedEmail && user.IsActive,
            cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return ApiResponse<AuthResponse>.Fail("Login failed", "Invalid email or password");
        }

        var response = CreateAuthResponse(user, dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<AuthResponse>.Ok(response, "Login successful");
    }

    public async Task<ApiResponse<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return ApiResponse<AuthResponse>.Fail("Validation failed", "Refresh token is required");
        }

        var refreshTokenHash = refreshTokenGenerator.Hash(request.RefreshToken);
        var now = dateTimeProvider.UtcNow;

        var persistedRefreshToken = await dbContext.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .SingleOrDefaultAsync(
                refreshToken =>
                    refreshToken.TokenHash == refreshTokenHash &&
                    refreshToken.RevokedAt == null &&
                    refreshToken.ExpiresAt > now &&
                    refreshToken.User.IsActive,
                cancellationToken);

        if (persistedRefreshToken is null)
        {
            return ApiResponse<AuthResponse>.Fail("Refresh failed", "Invalid refresh token");
        }

        persistedRefreshToken.RevokedAt = now;

        var response = CreateAuthResponse(persistedRefreshToken.User, now);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<AuthResponse>.Ok(response, "Token refreshed successfully");
    }

    public async Task<ApiResponse<UserResponse>> GetMeAsync(CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            return ApiResponse<UserResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == currentUserService.UserId && user.IsActive, cancellationToken);

        if (user is null)
        {
            return ApiResponse<UserResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        return ApiResponse<UserResponse>.Ok(ToUserResponse(user), "Success");
    }

    private AuthResponse CreateAuthResponse(User user, DateTime now)
    {
        var accessToken = jwtTokenService.CreateAccessToken(user);
        var refreshToken = refreshTokenGenerator.GenerateToken();

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenGenerator.Hash(refreshToken),
            ExpiresAt = now.AddDays(RefreshTokenDays),
            CreatedAt = now
        });

        return new AuthResponse(
            accessToken.AccessToken,
            refreshToken,
            accessToken.ExpiresIn,
            ToUserResponse(user));
    }

    private static UserResponse ToUserResponse(User user)
    {
        return new UserResponse(user.Id, user.Name, user.Email);
    }

    private static List<string> ValidateRegisterRequest(RegisterRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("Name is required");
        }
        else if (request.Name.Trim().Length > 120)
        {
            errors.Add("Name must be at most 120 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("Email is required");
        }
        else if (!request.Email.Contains('@', StringComparison.Ordinal) || request.Email.Trim().Length > 180)
        {
            errors.Add("Email is invalid");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add("Password is required");
        }
        else if (request.Password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters");
        }

        return errors;
    }

    private static List<string> ValidateLoginRequest(LoginRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("Email is required");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add("Password is required");
        }

        return errors;
    }
}

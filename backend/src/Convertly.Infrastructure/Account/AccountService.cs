using Convertly.Application.Account;
using Convertly.Application.Account.Dtos;
using Convertly.Application.Auth;
using Convertly.Application.Common;
using Convertly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Convertly.Infrastructure.Account;

public sealed class AccountService(
    ConvertlyDbContext dbContext,
    ICurrentUserService currentUserService,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    ILogger<AccountService> logger) : IAccountService
{
    private const int MinimumPasswordLength = 8;

    public async Task<ApiResponse<object>> ChangePasswordAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentActiveUserAsync(cancellationToken);
        if (user is null)
        {
            return ApiResponse<object>.Fail("Unauthorized", "Authenticated user was not found");
        }

        var validationErrors = ValidateChangePasswordRequest(request);
        if (validationErrors.Count > 0)
        {
            return ApiResponse<object>.Fail("Validation failed", validationErrors.ToArray());
        }

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            logger.LogWarning("account_password_change_denied user_id={UserId}", user.Id);
            return ApiResponse<object>.Fail("Password change failed", "Current password is invalid");
        }

        if (passwordHasher.Verify(request.NewPassword, user.PasswordHash))
        {
            return ApiResponse<object>.Fail("Validation failed", "New password must be different from current password");
        }

        var now = dateTimeProvider.UtcNow;
        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        user.UpdatedAt = now;

        await RevokeRefreshTokensAsync(user.Id, now, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("account_password_changed user_id={UserId}", user.Id);
        return ApiResponse<object>.Ok(new { }, "Password changed successfully");
    }

    public async Task<ApiResponse<object>> DeleteAccountAsync(
        DeleteAccountRequest request,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentActiveUserAsync(cancellationToken);
        if (user is null)
        {
            return ApiResponse<object>.Fail("Unauthorized", "Authenticated user was not found");
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            return ApiResponse<object>.Fail("Validation failed", "Current password is required");
        }

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            logger.LogWarning("account_delete_denied user_id={UserId}", user.Id);
            return ApiResponse<object>.Fail("Account deletion failed", "Current password is invalid");
        }

        var now = dateTimeProvider.UtcNow;
        user.IsActive = false;
        user.UpdatedAt = now;

        await RevokeRefreshTokensAsync(user.Id, now, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("account_deleted user_id={UserId}", user.Id);
        return ApiResponse<object>.Ok(new { }, "Account deleted successfully");
    }

    private async Task<Domain.Entities.User?> GetCurrentActiveUserAsync(CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            return null;
        }

        return await dbContext.Users.SingleOrDefaultAsync(
            user => user.Id == currentUserService.UserId && user.IsActive,
            cancellationToken);
    }

    private async Task RevokeRefreshTokensAsync(
        Guid userId,
        DateTime revokedAt,
        CancellationToken cancellationToken)
    {
        var activeRefreshTokens = await dbContext.RefreshTokens
            .Where(refreshToken => refreshToken.UserId == userId && refreshToken.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var refreshToken in activeRefreshTokens)
        {
            refreshToken.RevokedAt = revokedAt;
        }
    }

    private static List<string> ValidateChangePasswordRequest(ChangePasswordRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            errors.Add("Current password is required");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            errors.Add("New password is required");
        }
        else if (request.NewPassword.Length < MinimumPasswordLength)
        {
            errors.Add("New password must be at least 8 characters");
        }

        return errors;
    }
}

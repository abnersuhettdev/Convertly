using Convertly.Application.Auth;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Convertly.Api.Controllers;

[ApiController]
[Route("/auth")]
public sealed class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    [EnableRateLimiting(RateLimitPolicies.AuthSensitive)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken);
        if (response.Success && response.Data is not null)
        {
            logger.LogInformation("user_registered user_id={UserId}", response.Data.User.Id);
        }
        else
        {
            logger.LogWarning("user_registration_failed");
        }

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.AuthSensitive)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        if (response.Success && response.Data is not null)
        {
            logger.LogInformation("login_success user_id={UserId}", response.Data.User.Id);
        }
        else
        {
            logger.LogWarning("login_failed");
        }

        return response.Success ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting(RateLimitPolicies.AuthSensitive)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RefreshAsync(request, cancellationToken);

        return response.Success ? Ok(response) : Unauthorized(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Me(CancellationToken cancellationToken)
    {
        var response = await authService.GetMeAsync(cancellationToken);

        return response.Success ? Ok(response) : Unauthorized(response);
    }
}

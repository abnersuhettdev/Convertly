using Convertly.Application.Auth;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Convertly.Api.Controllers;

[ApiController]
[Route("/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);

        return response.Success ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("refresh")]
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

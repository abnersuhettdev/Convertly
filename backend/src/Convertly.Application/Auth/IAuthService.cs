using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;

namespace Convertly.Application.Auth;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<UserResponse>> GetMeAsync(CancellationToken cancellationToken);
}

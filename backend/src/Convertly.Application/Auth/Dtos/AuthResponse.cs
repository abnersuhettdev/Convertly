namespace Convertly.Application.Auth.Dtos;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserResponse User);

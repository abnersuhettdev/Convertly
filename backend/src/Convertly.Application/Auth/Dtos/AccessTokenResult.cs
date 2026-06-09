namespace Convertly.Application.Auth.Dtos;

public sealed record AccessTokenResult(
    string AccessToken,
    int ExpiresIn);

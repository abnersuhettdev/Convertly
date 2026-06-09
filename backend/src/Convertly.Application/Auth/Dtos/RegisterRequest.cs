namespace Convertly.Application.Auth.Dtos;

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password);

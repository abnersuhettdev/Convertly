namespace Convertly.Application.Auth.Dtos;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email);

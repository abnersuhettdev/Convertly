namespace Convertly.Application.Account.Dtos;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);

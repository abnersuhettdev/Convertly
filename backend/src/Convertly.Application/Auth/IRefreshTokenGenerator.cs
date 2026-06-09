namespace Convertly.Application.Auth;

public interface IRefreshTokenGenerator
{
    string GenerateToken();
    string Hash(string refreshToken);
}

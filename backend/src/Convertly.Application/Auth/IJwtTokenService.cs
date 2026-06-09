using Convertly.Application.Auth.Dtos;
using Convertly.Domain.Entities;

namespace Convertly.Application.Auth;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(User user);
}

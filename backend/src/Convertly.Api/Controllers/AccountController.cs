using Convertly.Application.Account;
using Convertly.Application.Account.Dtos;
using Convertly.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Convertly.Api.Controllers;

[ApiController]
[Authorize]
[Route("/account")]
public sealed class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPatch("password")]
    [EnableRateLimiting(RateLimitPolicies.AccountSensitive)]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await accountService.ChangePasswordAsync(request, cancellationToken);

        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete]
    [EnableRateLimiting(RateLimitPolicies.AccountSensitive)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAccount(
        DeleteAccountRequest request,
        CancellationToken cancellationToken)
    {
        var response = await accountService.DeleteAccountAsync(request, cancellationToken);

        return response.Success ? Ok(response) : BadRequest(response);
    }
}

using Convertly.Application.Common;
using Convertly.Application.Subscriptions;
using Convertly.Application.Subscriptions.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Convertly.Api.Controllers;

[ApiController]
[Authorize]
[Route("/subscription")]
public sealed class SubscriptionController(ISubscriptionService subscriptionService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<SubscriptionResponse>>> Me(CancellationToken cancellationToken)
    {
        var response = await subscriptionService.GetCurrentSubscriptionAsync(cancellationToken);

        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("change-plan")]
    public async Task<ActionResult<ApiResponse<SubscriptionResponse>>> ChangePlan(
        ChangePlanRequest request,
        CancellationToken cancellationToken)
    {
        var response = await subscriptionService.ChangePlanAsync(request, cancellationToken);

        return response.Success ? Ok(response) : BadRequest(response);
    }
}

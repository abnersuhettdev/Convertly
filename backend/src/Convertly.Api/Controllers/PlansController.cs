using Convertly.Application.Common;
using Convertly.Application.Subscriptions;
using Convertly.Application.Subscriptions.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Convertly.Api.Controllers;

[ApiController]
[Route("/plans")]
public sealed class PlansController(IPlanService planService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PlanResponse>>>> GetPlans(CancellationToken cancellationToken)
    {
        var response = await planService.GetPlansAsync(cancellationToken);

        return Ok(response);
    }
}

using Convertly.Application.Common;
using Convertly.Application.Subscriptions;
using Convertly.Application.Subscriptions.Dtos;
using Convertly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Convertly.Infrastructure.Subscriptions;

public sealed class PlanService(ConvertlyDbContext dbContext) : IPlanService
{
    public async Task<ApiResponse<IReadOnlyList<PlanResponse>>> GetPlansAsync(CancellationToken cancellationToken)
    {
        var plans = await dbContext.Plans
            .AsNoTracking()
            .OrderBy(plan => plan.PriceCents)
            .Select(plan => PlanMapping.ToResponse(plan))
            .ToListAsync(cancellationToken);

        return ApiResponse<IReadOnlyList<PlanResponse>>.Ok(plans, "Success");
    }
}

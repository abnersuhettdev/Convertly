using Convertly.Application.Common;
using Convertly.Application.Subscriptions.Dtos;

namespace Convertly.Application.Subscriptions;

public interface IPlanService
{
    Task<ApiResponse<IReadOnlyList<PlanResponse>>> GetPlansAsync(CancellationToken cancellationToken);
}

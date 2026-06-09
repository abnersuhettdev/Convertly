using Convertly.Application.Common;
using Convertly.Application.Subscriptions.Dtos;

namespace Convertly.Application.Subscriptions;

public interface ISubscriptionService
{
    Task<ApiResponse<SubscriptionResponse>> GetCurrentSubscriptionAsync(CancellationToken cancellationToken);
    Task<ApiResponse<SubscriptionResponse>> ChangePlanAsync(ChangePlanRequest request, CancellationToken cancellationToken);
}

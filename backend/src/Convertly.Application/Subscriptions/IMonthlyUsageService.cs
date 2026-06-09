using Convertly.Application.Common;
using Convertly.Application.Subscriptions.Dtos;

namespace Convertly.Application.Subscriptions;

public interface IMonthlyUsageService
{
    Task<MonthlyUsageSummary> GetOrCreateCurrentMonthUsageAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<MonthlyUsageSummary>> ReserveConversionAsync(Guid userId, CancellationToken cancellationToken);
    Task<ApiResponse<MonthlyUsageSummary>> ReturnConversionAsync(Guid userId, CancellationToken cancellationToken);
}

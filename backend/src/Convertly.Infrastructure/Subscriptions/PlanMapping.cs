using Convertly.Application.Subscriptions.Dtos;
using Convertly.Domain.Entities;

namespace Convertly.Infrastructure.Subscriptions;

internal static class PlanMapping
{
    public static PlanResponse ToResponse(Plan plan)
    {
        return new PlanResponse(
            plan.Id,
            plan.Name,
            plan.Slug,
            plan.MonthlyConversionLimit,
            plan.MaxFileSizeMb,
            plan.RetentionHours,
            plan.PriceCents,
            plan.IsActive);
    }
}

namespace Convertly.Application.Subscriptions.Dtos;

public sealed record PlanResponse(
    Guid Id,
    string Name,
    string Slug,
    int MonthlyConversionLimit,
    int MaxFileSizeMb,
    int RetentionHours,
    int PriceCents,
    bool IsActive);

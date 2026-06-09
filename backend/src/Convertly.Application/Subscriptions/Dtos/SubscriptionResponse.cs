namespace Convertly.Application.Subscriptions.Dtos;

public sealed record SubscriptionResponse(
    PlanResponse Plan,
    int MonthlyLimit,
    int ConversionsUsed,
    int ConversionsRemaining,
    int MaxFileSizeMb,
    int RetentionHours);

namespace Convertly.Application.Subscriptions.Dtos;

public sealed record MonthlyUsageSummary(
    Guid UserId,
    int Year,
    int Month,
    int ConversionsUsed);

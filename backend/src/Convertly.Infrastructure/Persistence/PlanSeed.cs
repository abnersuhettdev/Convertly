using Convertly.Domain.Constants;
using Convertly.Domain.Entities;

namespace Convertly.Infrastructure.Persistence;

public static class PlanSeed
{
    public static readonly Guid FreePlanId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ProPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid BusinessPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly DateTime SeedCreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<Plan> Plans { get; } =
    [
        new()
        {
            Id = FreePlanId,
            Name = "Free",
            Slug = PlanSlugs.Free,
            MonthlyConversionLimit = 5,
            MaxFileSizeMb = 10,
            RetentionHours = 24,
            PriceCents = 0,
            IsActive = true,
            CreatedAt = SeedCreatedAt
        },
        new()
        {
            Id = ProPlanId,
            Name = "Pro",
            Slug = PlanSlugs.Pro,
            MonthlyConversionLimit = 100,
            MaxFileSizeMb = 50,
            RetentionHours = 168,
            PriceCents = 1990,
            IsActive = true,
            CreatedAt = SeedCreatedAt
        },
        new()
        {
            Id = BusinessPlanId,
            Name = "Business",
            Slug = PlanSlugs.Business,
            MonthlyConversionLimit = 500,
            MaxFileSizeMb = 200,
            RetentionHours = 720,
            PriceCents = 4990,
            IsActive = true,
            CreatedAt = SeedCreatedAt
        }
    ];
}

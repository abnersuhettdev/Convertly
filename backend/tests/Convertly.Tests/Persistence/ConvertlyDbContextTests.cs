using Convertly.Domain.Constants;
using Convertly.Domain.Entities;
using Convertly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convertly.Tests.Persistence;

public sealed class ConvertlyDbContextTests
{
    [Fact]
    public void OfficialPlansSeedContainsExpectedPlans()
    {
        var plans = PlanSeed.Plans.ToDictionary(plan => plan.Slug);

        Assert.Equal(3, plans.Count);

        AssertPlan(plans[PlanSlugs.Free], "Free", 5, 10, 24, 0);
        AssertPlan(plans[PlanSlugs.Pro], "Pro", 100, 50, 168, 1990);
        AssertPlan(plans[PlanSlugs.Business], "Business", 500, 200, 720, 4990);
    }

    [Fact]
    public void ModelConfiguresExpectedUniqueIndexes()
    {
        var options = new DbContextOptionsBuilder<ConvertlyDbContext>()
            .UseNpgsql("Host=localhost;Database=convertly;Username=postgres;Password=postgres")
            .Options;

        using var dbContext = new ConvertlyDbContext(options);

        var userEntity = dbContext.Model.FindEntityType(typeof(User));
        var planEntity = dbContext.Model.FindEntityType(typeof(Plan));
        var monthlyUsageEntity = dbContext.Model.FindEntityType(typeof(MonthlyUsage));

        Assert.NotNull(userEntity?.FindIndex(userEntity.FindProperty(nameof(User.Email))!));
        Assert.True(userEntity.FindIndex(userEntity.FindProperty(nameof(User.Email))!)?.IsUnique);

        Assert.NotNull(planEntity?.FindIndex(planEntity.FindProperty(nameof(Plan.Slug))!));
        Assert.True(planEntity.FindIndex(planEntity.FindProperty(nameof(Plan.Slug))!)?.IsUnique);

        var monthlyUsageIndex = monthlyUsageEntity?.GetIndexes().SingleOrDefault(index =>
            index.Properties.Select(property => property.Name).SequenceEqual(
            [
                nameof(MonthlyUsage.UserId),
                nameof(MonthlyUsage.Year),
                nameof(MonthlyUsage.Month)
            ]));

        Assert.NotNull(monthlyUsageIndex);
        Assert.True(monthlyUsageIndex.IsUnique);
    }

    private static void AssertPlan(
        Plan plan,
        string name,
        int monthlyConversionLimit,
        int maxFileSizeMb,
        int retentionHours,
        int priceCents)
    {
        Assert.Equal(name, plan.Name);
        Assert.Equal(monthlyConversionLimit, plan.MonthlyConversionLimit);
        Assert.Equal(maxFileSizeMb, plan.MaxFileSizeMb);
        Assert.Equal(retentionHours, plan.RetentionHours);
        Assert.Equal(priceCents, plan.PriceCents);
        Assert.True(plan.IsActive);
    }
}

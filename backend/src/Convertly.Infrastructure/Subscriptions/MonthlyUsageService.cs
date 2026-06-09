using Convertly.Application.Common;
using Convertly.Application.Subscriptions;
using Convertly.Application.Subscriptions.Dtos;
using Convertly.Domain.Entities;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Convertly.Infrastructure.Subscriptions;

public sealed class MonthlyUsageService(
    ConvertlyDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IMonthlyUsageService
{
    public async Task<MonthlyUsageSummary> GetOrCreateCurrentMonthUsageAsync(Guid userId, CancellationToken cancellationToken)
    {
        var monthlyUsage = await GetOrCreateMonthlyUsageEntityAsync(userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToSummary(monthlyUsage);
    }

    public async Task<ApiResponse<MonthlyUsageSummary>> ReserveConversionAsync(Guid userId, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var activeSubscription = await GetActiveSubscriptionAsync(userId, cancellationToken);
        if (activeSubscription is null)
        {
            return ApiResponse<MonthlyUsageSummary>.Fail("Usage reservation failed", "Active subscription was not found");
        }

        if (!activeSubscription.Plan.IsActive)
        {
            return ApiResponse<MonthlyUsageSummary>.Fail("Usage reservation failed", "Current plan is inactive");
        }

        var monthlyUsage = await GetOrCreateMonthlyUsageEntityAsync(userId, cancellationToken);

        if (monthlyUsage.ConversionsUsed >= activeSubscription.Plan.MonthlyConversionLimit)
        {
            return ApiResponse<MonthlyUsageSummary>.Fail("Monthly limit reached", "Monthly conversion limit reached");
        }

        monthlyUsage.ConversionsUsed += 1;
        monthlyUsage.UpdatedAt = dateTimeProvider.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ApiResponse<MonthlyUsageSummary>.Ok(ToSummary(monthlyUsage), "Usage reserved successfully");
    }

    public async Task<ApiResponse<MonthlyUsageSummary>> ReturnConversionAsync(Guid userId, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var monthlyUsage = await GetOrCreateMonthlyUsageEntityAsync(userId, cancellationToken);
        monthlyUsage.ConversionsUsed = Math.Max(0, monthlyUsage.ConversionsUsed - 1);
        monthlyUsage.UpdatedAt = dateTimeProvider.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ApiResponse<MonthlyUsageSummary>.Ok(ToSummary(monthlyUsage), "Usage returned successfully");
    }

    private async Task<UserSubscription?> GetActiveSubscriptionAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserSubscriptions
            .Include(subscription => subscription.Plan)
            .Where(subscription => subscription.UserId == userId && subscription.Status == SubscriptionStatus.Active)
            .OrderByDescending(subscription => subscription.StartedAt)
            .ThenByDescending(subscription => subscription.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<MonthlyUsage> GetOrCreateMonthlyUsageEntityAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var monthlyUsage = await dbContext.MonthlyUsages.SingleOrDefaultAsync(
            usage => usage.UserId == userId && usage.Year == now.Year && usage.Month == now.Month,
            cancellationToken);

        if (monthlyUsage is not null)
        {
            return monthlyUsage;
        }

        monthlyUsage = new MonthlyUsage
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Year = now.Year,
            Month = now.Month,
            ConversionsUsed = 0,
            CreatedAt = now
        };

        dbContext.MonthlyUsages.Add(monthlyUsage);
        return monthlyUsage;
    }

    private static MonthlyUsageSummary ToSummary(MonthlyUsage monthlyUsage)
    {
        return new MonthlyUsageSummary(
            monthlyUsage.UserId,
            monthlyUsage.Year,
            monthlyUsage.Month,
            monthlyUsage.ConversionsUsed);
    }
}

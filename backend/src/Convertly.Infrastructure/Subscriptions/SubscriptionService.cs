using Convertly.Application.Auth;
using Convertly.Application.Common;
using Convertly.Application.Subscriptions;
using Convertly.Application.Subscriptions.Dtos;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Convertly.Infrastructure.Subscriptions;

public sealed class SubscriptionService(
    ConvertlyDbContext dbContext,
    ICurrentUserService currentUserService,
    IMonthlyUsageService monthlyUsageService,
    IDateTimeProvider dateTimeProvider) : ISubscriptionService
{
    public async Task<ApiResponse<SubscriptionResponse>> GetCurrentSubscriptionAsync(CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();
        if (userId is null)
        {
            return ApiResponse<SubscriptionResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        var activeSubscription = await GetActiveSubscriptionAsync(userId.Value, cancellationToken);
        if (activeSubscription is null)
        {
            return ApiResponse<SubscriptionResponse>.Fail("Subscription not found", "Active subscription was not found");
        }

        var usage = await monthlyUsageService.GetOrCreateCurrentMonthUsageAsync(userId.Value, cancellationToken);

        return ApiResponse<SubscriptionResponse>.Ok(CreateResponse(activeSubscription.Plan, usage), "Success");
    }

    public async Task<ApiResponse<SubscriptionResponse>> ChangePlanAsync(ChangePlanRequest request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();
        if (userId is null)
        {
            return ApiResponse<SubscriptionResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        if (string.IsNullOrWhiteSpace(request.PlanSlug))
        {
            return ApiResponse<SubscriptionResponse>.Fail("Validation failed", "Plan slug is required");
        }

        var normalizedPlanSlug = request.PlanSlug.Trim().ToLowerInvariant();
        var targetPlan = await dbContext.Plans.SingleOrDefaultAsync(plan => plan.Slug == normalizedPlanSlug, cancellationToken);

        if (targetPlan is null)
        {
            return ApiResponse<SubscriptionResponse>.Fail("Plan change failed", "Plan was not found");
        }

        if (!targetPlan.IsActive)
        {
            return ApiResponse<SubscriptionResponse>.Fail("Plan change failed", "Plan is inactive");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var activeSubscriptions = await dbContext.UserSubscriptions
            .Where(subscription => subscription.UserId == userId.Value && subscription.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var subscription in activeSubscriptions)
        {
            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.EndsAt = now;
            subscription.UpdatedAt = now;
        }

        dbContext.UserSubscriptions.Add(new()
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            PlanId = targetPlan.Id,
            Status = SubscriptionStatus.Active,
            StartedAt = now,
            CreatedAt = now
        });

        var usage = await monthlyUsageService.GetOrCreateCurrentMonthUsageAsync(userId.Value, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ApiResponse<SubscriptionResponse>.Ok(CreateResponse(targetPlan, usage), "Plan changed successfully");
    }

    private Guid? GetAuthenticatedUserId()
    {
        return currentUserService.IsAuthenticated ? currentUserService.UserId : null;
    }

    private async Task<Domain.Entities.UserSubscription?> GetActiveSubscriptionAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.UserSubscriptions
            .AsNoTracking()
            .Include(subscription => subscription.Plan)
            .Where(subscription => subscription.UserId == userId && subscription.Status == SubscriptionStatus.Active)
            .OrderByDescending(subscription => subscription.StartedAt)
            .ThenByDescending(subscription => subscription.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static SubscriptionResponse CreateResponse(Domain.Entities.Plan plan, MonthlyUsageSummary usage)
    {
        var remaining = Math.Max(0, plan.MonthlyConversionLimit - usage.ConversionsUsed);

        return new SubscriptionResponse(
            PlanMapping.ToResponse(plan),
            plan.MonthlyConversionLimit,
            usage.ConversionsUsed,
            remaining,
            plan.MaxFileSizeMb,
            plan.RetentionHours);
    }
}

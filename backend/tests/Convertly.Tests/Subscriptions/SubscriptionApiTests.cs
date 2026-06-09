using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Convertly.Application.Subscriptions;
using Convertly.Application.Subscriptions.Dtos;
using Convertly.Domain.Constants;
using Convertly.Infrastructure.Persistence;
using Convertly.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Convertly.Tests.Subscriptions;

public sealed class SubscriptionApiTests
{
    [Fact]
    public async Task GetPlans_ReturnsOfficialPlans()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/plans");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<PlanResponse>>>();
        var planSlugs = body?.Data?.Select(plan => plan.Slug).ToHashSet() ?? [];

        Assert.True(body?.Success);
        Assert.Contains(PlanSlugs.Free, planSlugs);
        Assert.Contains(PlanSlugs.Pro, planSlugs);
        Assert.Contains(PlanSlugs.Business, planSlugs);
    }

    [Fact]
    public async Task SubscriptionMe_WithAuthenticatedUser_ReturnsSubscription()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.GetAsync("/api/subscription/me");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SubscriptionResponse>>();

        Assert.True(body?.Success);
        Assert.NotNull(body?.Data?.Plan);
        Assert.Equal(0, body?.Data?.ConversionsUsed);
    }

    [Fact]
    public async Task NewUser_HasFreePlan()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.GetAsync("/api/subscription/me");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SubscriptionResponse>>();

        Assert.Equal(PlanSlugs.Free, body?.Data?.Plan.Slug);
        Assert.Equal(5, body?.Data?.MonthlyLimit);
        Assert.Equal(5, body?.Data?.ConversionsRemaining);
        Assert.Equal(10, body?.Data?.MaxFileSizeMb);
        Assert.Equal(24, body?.Data?.RetentionHours);
    }

    [Fact]
    public async Task ChangePlan_ToPro_Works()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsJsonAsync("/api/subscription/change-plan", new ChangePlanRequest(PlanSlugs.Pro));

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SubscriptionResponse>>();

        Assert.Equal(PlanSlugs.Pro, body?.Data?.Plan.Slug);
        Assert.Equal(100, body?.Data?.MonthlyLimit);
    }

    [Fact]
    public async Task ChangePlan_ToBusiness_Works()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsJsonAsync("/api/subscription/change-plan", new ChangePlanRequest(PlanSlugs.Business));

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SubscriptionResponse>>();

        Assert.Equal(PlanSlugs.Business, body?.Data?.Plan.Slug);
        Assert.Equal(500, body?.Data?.MonthlyLimit);
    }

    [Fact]
    public async Task ChangePlan_WithUnknownPlan_Fails()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsJsonAsync("/api/subscription/change-plan", new ChangePlanRequest("unknown"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangePlan_DowngradeBelowCurrentUsage_ReturnsZeroRemainingConversions()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        await client.PostAsJsonAsync("/api/subscription/change-plan", new ChangePlanRequest(PlanSlugs.Pro));

        await using var scope = factory.Services.CreateAsyncScope();
        var monthlyUsageService = scope.ServiceProvider.GetRequiredService<IMonthlyUsageService>();
        for (var index = 0; index < 6; index++)
        {
            var reservation = await monthlyUsageService.ReserveConversionAsync(authResponse.User.Id, CancellationToken.None);
            Assert.True(reservation.Success);
        }

        var downgrade = await client.PostAsJsonAsync("/api/subscription/change-plan", new ChangePlanRequest(PlanSlugs.Free));

        downgrade.EnsureSuccessStatusCode();
        var body = await downgrade.Content.ReadFromJsonAsync<ApiResponse<SubscriptionResponse>>();

        Assert.Equal(PlanSlugs.Free, body?.Data?.Plan.Slug);
        Assert.Equal(6, body?.Data?.ConversionsUsed);
        Assert.Equal(0, body?.Data?.ConversionsRemaining);
    }

    [Fact]
    public async Task SubscriptionMe_CreatesMonthlyUsage_WhenMissing()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        var response = await client.GetAsync("/api/subscription/me");

        response.EnsureSuccessStatusCode();
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var usage = await dbContext.MonthlyUsages.SingleAsync(usage => usage.UserId == authResponse.User.Id);

        Assert.Equal(0, usage.ConversionsUsed);
    }

    [Fact]
    public async Task ReserveConversion_IncrementsConversionsUsed()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        await using var scope = factory.Services.CreateAsyncScope();
        var monthlyUsageService = scope.ServiceProvider.GetRequiredService<IMonthlyUsageService>();

        var response = await monthlyUsageService.ReserveConversionAsync(authResponse.User.Id, CancellationToken.None);

        Assert.True(response.Success);
        Assert.Equal(1, response.Data?.ConversionsUsed);
    }

    [Fact]
    public async Task ReserveConversion_Blocks_WhenLimitIsReached()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        await using var scope = factory.Services.CreateAsyncScope();
        var monthlyUsageService = scope.ServiceProvider.GetRequiredService<IMonthlyUsageService>();

        for (var index = 0; index < 5; index++)
        {
            var reservation = await monthlyUsageService.ReserveConversionAsync(authResponse.User.Id, CancellationToken.None);
            Assert.True(reservation.Success);
        }

        var blockedReservation = await monthlyUsageService.ReserveConversionAsync(authResponse.User.Id, CancellationToken.None);

        Assert.False(blockedReservation.Success);
    }

    [Fact]
    public async Task ReturnConversion_DecrementsWithoutGoingNegative()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        await using var scope = factory.Services.CreateAsyncScope();
        var monthlyUsageService = scope.ServiceProvider.GetRequiredService<IMonthlyUsageService>();

        var emptyReturn = await monthlyUsageService.ReturnConversionAsync(authResponse.User.Id, CancellationToken.None);
        await monthlyUsageService.ReserveConversionAsync(authResponse.User.Id, CancellationToken.None);
        await monthlyUsageService.ReserveConversionAsync(authResponse.User.Id, CancellationToken.None);
        var returnResponse = await monthlyUsageService.ReturnConversionAsync(authResponse.User.Id, CancellationToken.None);

        Assert.Equal(0, emptyReturn.Data?.ConversionsUsed);
        Assert.Equal(1, returnResponse.Data?.ConversionsUsed);
    }

    [Theory]
    [InlineData("/api/subscription/me", "GET")]
    [InlineData("/api/subscription/change-plan", "POST")]
    public async Task PrivateSubscriptionEndpoints_WithoutToken_ReturnUnauthorized(string url, string method)
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = method == "GET"
            ? await client.GetAsync(url)
            : await client.PostAsJsonAsync(url, new ChangePlanRequest(PlanSlugs.Pro));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<AuthResponse> RegisterAndAuthorizeAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("Abner Suhett", $"abner-{Guid.NewGuid():N}@example.com", "StrongPassword123!"));

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        var authResponse = body?.Data ?? throw new InvalidOperationException("Auth response was empty.");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        return authResponse;
    }
}

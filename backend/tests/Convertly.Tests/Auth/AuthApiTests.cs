using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Convertly.Domain.Constants;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Convertly.Tests.Auth;

public sealed class AuthApiTests
{
    [Fact]
    public async Task Register_WithValidRequest_CreatesUser()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest());

        response.EnsureSuccessStatusCode();
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var user = await dbContext.Users.SingleAsync();

        Assert.Equal("abner@example.com", user.Email);
        Assert.NotEqual("StrongPassword123!", user.PasswordHash);
    }

    [Fact]
    public async Task Register_WithValidRequest_CreatesFreeSubscription()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest());

        response.EnsureSuccessStatusCode();
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var subscription = await dbContext.UserSubscriptions
            .Include(subscription => subscription.Plan)
            .SingleAsync();

        Assert.Equal(PlanSlugs.Free, subscription.Plan.Slug);
        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Fails()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest());
        var response = await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest(email: "ABNER@example.com"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        Assert.False(body?.Success);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest());
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("abner@example.com", "StrongPassword123!"));

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

        Assert.True(body?.Success);
        Assert.False(string.IsNullOrWhiteSpace(body?.Data?.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(body?.Data?.RefreshToken));
        Assert.Equal(3600, body?.Data?.ExpiresIn);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Fails()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest());
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("abner@example.com", "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithAuthentication_ReturnsAuthenticatedUser()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var registerResponse = await RegisterAndReadAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", registerResponse.AccessToken);

        var response = await client.GetAsync("/api/auth/me");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
        Assert.Equal(registerResponse.User.Id, body?.Data?.Id);
        Assert.Equal("abner@example.com", body?.Data?.Email);
    }

    [Fact]
    public async Task Refresh_WithValidRefreshToken_ReturnsNewTokens()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var authResponse = await RegisterAndReadAsync(client);
        var response = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest(authResponse.RefreshToken));

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

        Assert.True(body?.Success);
        Assert.False(string.IsNullOrWhiteSpace(body?.Data?.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(body?.Data?.RefreshToken));
        Assert.NotEqual(authResponse.RefreshToken, body?.Data?.RefreshToken);
    }

    [Fact]
    public async Task Refresh_RotatesRefreshToken_AndRevokesPreviousToken()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var authResponse = await RegisterAndReadAsync(client);
        var firstRefresh = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest(authResponse.RefreshToken));
        firstRefresh.EnsureSuccessStatusCode();

        var reusedRefresh = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest(authResponse.RefreshToken));

        Assert.Equal(HttpStatusCode.Unauthorized, reusedRefresh.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithInvalidRefreshToken_Fails()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest("invalid-token"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static RegisterRequest ValidRegisterRequest(string email = "Abner@example.com")
    {
        return new RegisterRequest("Abner Suhett", email, "StrongPassword123!");
    }

    private static async Task<AuthResponse> RegisterAndReadAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", ValidRegisterRequest());
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

        return body?.Data ?? throw new InvalidOperationException("Auth response was empty.");
    }
}

internal sealed class ConvertlyApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly Action<IServiceCollection>? _configureServices;

    public ConvertlyApiFactory(Action<IServiceCollection>? configureServices = null)
    {
        _configureServices = configureServices;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Database=convertly;Username=postgres;Password=postgres");
        builder.UseSetting("Jwt:Secret", "testing-secret-change-me-testing-secret-32");
        builder.UseSetting("Jwt:Issuer", "Convertly");
        builder.UseSetting("Jwt:Audience", "ConvertlyUsers");
        builder.UseSetting("Jwt:AccessTokenMinutes", "60");
        builder.UseSetting("Jwt:RefreshTokenDays", "7");
        builder.UseSetting("Supabase:Url", "https://your-project.supabase.co");
        builder.UseSetting("Supabase:ServiceRoleKey", "development-service-role-key");
        builder.UseSetting("Supabase:OriginalsBucket", "convertly-originals");
        builder.UseSetting("Supabase:ConvertedBucket", "convertly-converted");

        builder.ConfigureServices(services =>
        {
            services.Configure<HostOptions>(options =>
            {
                options.ShutdownTimeout = TimeSpan.FromSeconds(1);
            });

            services.RemoveAll<DbContextOptions<ConvertlyDbContext>>();

            _connection.Open();

            services.AddDbContext<ConvertlyDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            _configureServices?.Invoke(services);

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}

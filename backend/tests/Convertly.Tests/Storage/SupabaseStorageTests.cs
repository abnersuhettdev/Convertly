using System.Net.Http.Json;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Files;
using Convertly.Infrastructure;
using Convertly.Infrastructure.Storage;
using Convertly.Tests.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Convertly.Tests.Storage;

public sealed class SupabaseStorageTests
{
    [Fact]
    public void SupabaseOptions_LoadsExpectedConfiguration()
    {
        var configuration = CreateConfiguration();
        var options = configuration.GetSection("Supabase").Get<SupabaseStorageOptions>();

        Assert.NotNull(options);
        Assert.Equal("https://your-project.supabase.co", options.Url);
        Assert.Equal("development-service-role-key", options.ServiceRoleKey);
        Assert.Equal("convertly-originals", options.OriginalsBucket);
        Assert.Equal("convertly-converted", options.ConvertedBucket);
    }

    [Fact]
    public void SupabaseOptions_MissingRequiredConfiguration_FailsClearly()
    {
        var options = new SupabaseStorageOptions
        {
            Url = "https://your-project.supabase.co",
            OriginalsBucket = "convertly-originals",
            ConvertedBucket = "convertly-converted"
        };

        var exception = Assert.Throws<InvalidOperationException>(options.Validate);

        Assert.Contains("Supabase__ServiceRoleKey", exception.Message);
    }

    [Fact]
    public void BuildOriginalPath_ReturnsExpectedFormat()
    {
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var conversionId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var fileId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        var path = SupabaseStoragePathBuilder.BuildOriginalPath(userId, conversionId, fileId);

        Assert.Equal(
            "users/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/originals/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb/cccccccc-cccc-cccc-cccc-cccccccccccc.docx",
            path);
    }

    [Fact]
    public void BuildConvertedPath_ReturnsExpectedFormat()
    {
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var conversionId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var fileId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        var path = SupabaseStoragePathBuilder.BuildConvertedPath(userId, conversionId, fileId);

        Assert.Equal(
            "users/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/converted/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb/cccccccc-cccc-cccc-cccc-cccccccccccc.pdf",
            path);
    }

    [Fact]
    public void FileStorageService_IsRegisteredInDependencyInjection()
    {
        var services = new ServiceCollection();
        services.AddInfrastructure(CreateConfiguration());
        using var provider = services.BuildServiceProvider();

        var storageService = provider.GetRequiredService<IFileStorageService>();

        Assert.IsType<SupabaseFileStorageService>(storageService);
    }

    [Fact]
    public async Task ExistingResponses_DoNotExposeSupabaseServiceRoleKey()
    {
        using var factory = new ConvertlyApiFactory();
        using var client = factory.CreateClient();

        var plansResponse = await client.GetAsync("/api/plans");
        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("Abner Suhett", $"abner-{Guid.NewGuid():N}@example.com", "StrongPassword123!"));

        var plansBody = await plansResponse.Content.ReadAsStringAsync();
        var registerBody = await registerResponse.Content.ReadAsStringAsync();

        Assert.DoesNotContain("development-service-role-key", plansBody);
        Assert.DoesNotContain("development-service-role-key", registerBody);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=convertly;Username=postgres;Password=postgres",
                ["Jwt:Secret"] = "testing-secret-change-me-testing-secret-32",
                ["Jwt:Issuer"] = "Convertly",
                ["Jwt:Audience"] = "ConvertlyUsers",
                ["Jwt:AccessTokenMinutes"] = "60",
                ["Jwt:RefreshTokenDays"] = "7",
                ["Supabase:Url"] = "https://your-project.supabase.co",
                ["Supabase:ServiceRoleKey"] = "development-service-role-key",
                ["Supabase:OriginalsBucket"] = "convertly-originals",
                ["Supabase:ConvertedBucket"] = "convertly-converted"
            })
            .Build();
    }
}

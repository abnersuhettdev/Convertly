using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Convertly.Application.Conversions;
using Convertly.Application.Conversions.Dtos;
using Convertly.Application.Files;
using Convertly.Domain.Constants;
using Convertly.Domain.Entities;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Convertly.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Convertly.Tests.Conversions;

public sealed class ConversionHistoryApiTests
{
    [Fact]
    public async Task GetConversions_WithoutToken_ReturnsUnauthorized()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/conversions");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetConversions_ReturnsOnlyAuthenticatedUserConversions()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var firstUser = await RegisterAndAuthorizeAsync(client);
        await CreateConversionAsync(factory, firstUser.User.Id, ConversionStatus.Completed);
        await CreateConversionAsync(factory, Guid.NewGuid(), ConversionStatus.Completed);

        var response = await client.GetAsync("/api/conversions");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ConversionListResponse>>();
        Assert.Single(body!.Data!.Items);
    }

    [Fact]
    public async Task GetConversions_AppliesPagination()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Pending, createdAt: DateTime.UtcNow.AddMinutes(-3));
        await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Processing, createdAt: DateTime.UtcNow.AddMinutes(-2));
        await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Completed, createdAt: DateTime.UtcNow.AddMinutes(-1));

        var response = await client.GetAsync("/api/conversions?page=2&pageSize=2");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ConversionListResponse>>();
        Assert.Equal(2, body!.Data!.Page);
        Assert.Equal(2, body.Data.PageSize);
        Assert.Equal(3, body.Data.TotalItems);
        Assert.Equal(2, body.Data.TotalPages);
        Assert.Single(body.Data.Items);
    }

    [Fact]
    public async Task GetConversions_FiltersByStatus()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Completed);
        await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Failed);

        var response = await client.GetAsync("/api/conversions?status=Completed");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ConversionListResponse>>();
        Assert.Single(body!.Data!.Items);
        Assert.Equal(ConversionStatus.Completed.ToString(), body.Data.Items[0].Status);
    }

    [Fact]
    public async Task GetConversions_WithInvalidStatus_ReturnsBadRequest()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.GetAsync("/api/conversions?status=Unknown");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetConversion_ReturnsOwnedConversionDetail()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        var conversionId = await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Completed);

        var response = await client.GetAsync($"/api/conversions/{conversionId}");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ConversionDetailResponse>>();
        Assert.Equal(conversionId, body!.Data!.Id);
        Assert.True(body.Data.DownloadAvailable);
    }

    [Fact]
    public async Task GetConversion_DoesNotReturnAnotherUsersConversion()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);
        var conversionId = await CreateConversionAsync(factory, Guid.NewGuid(), ConversionStatus.Completed);

        var response = await client.GetAsync($"/api/conversions/{conversionId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Download_WithoutToken_ReturnsUnauthorized()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/conversions/{Guid.NewGuid()}/download");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Download_DoesNotReturnAnotherUsersConversion()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);
        var conversionId = await CreateConversionAsync(factory, Guid.NewGuid(), ConversionStatus.Completed);

        var response = await client.GetAsync($"/api/conversions/{conversionId}/download");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(ConversionStatus.Pending)]
    [InlineData(ConversionStatus.Processing)]
    [InlineData(ConversionStatus.Failed)]
    public async Task Download_BlocksUnavailableStatuses(ConversionStatus status)
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        var conversionId = await CreateConversionAsync(factory, auth.User.Id, status);

        var response = await client.GetAsync($"/api/conversions/{conversionId}/download");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Download_CompletedConversion_ReturnsPdf()
    {
        var fakeStorage = new FakeFileStorageService { DownloadBytes = [37, 80, 68, 70] };
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        var conversionId = await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Completed);

        var response = await client.GetAsync($"/api/conversions/{conversionId}/download");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("document.pdf", response.Content.Headers.ContentDisposition?.FileName ?? string.Empty);
        Assert.Equal(fakeStorage.DownloadBytes, await response.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task Download_BlocksExpiredConversion()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        var conversionId = await CreateConversionAsync(
            factory,
            auth.User.Id,
            ConversionStatus.Completed,
            expiresAt: DateTime.UtcNow.AddHours(-1));

        var response = await client.GetAsync($"/api/conversions/{conversionId}/download");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetConversions_ResponseDoesNotExposeStorageFields()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Completed);

        var response = await client.GetAsync("/api/conversions");
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("bucketName", responseBody);
        Assert.DoesNotContain("storagePath", responseBody);
    }

    [Fact]
    public async Task GetConversion_ResponseDoesNotExposeStorageFields()
    {
        using var factory = CreateFactory(new FakeFileStorageService());
        using var client = factory.CreateClient();
        var auth = await RegisterAndAuthorizeAsync(client);
        var conversionId = await CreateConversionAsync(factory, auth.User.Id, ConversionStatus.Completed);

        var response = await client.GetAsync($"/api/conversions/{conversionId}");
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("bucketName", responseBody);
        Assert.DoesNotContain("storagePath", responseBody);
    }

    private static ConvertlyApiFactory CreateFactory(FakeFileStorageService fakeStorage)
    {
        return new ConvertlyApiFactory(services =>
        {
            services.RemoveAll<IFileStorageService>();
            services.RemoveAll<IConversionJobQueue>();
            services.AddSingleton<IFileStorageService>(fakeStorage);
            services.AddSingleton<IConversionJobQueue, NoOpConversionJobQueue>();
        });
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

    private static async Task<Guid> CreateConversionAsync(
        ConvertlyApiFactory factory,
        Guid userId,
        ConversionStatus status,
        DateTime? createdAt = null,
        DateTime? expiresAt = null)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var now = createdAt ?? DateTime.UtcNow;
        var conversionId = Guid.NewGuid();
        var sourceFileId = Guid.NewGuid();
        Guid? outputFileId = status == ConversionStatus.Completed ? Guid.NewGuid() : null;

        if (!await dbContext.Users.AnyAsync(user => user.Id == userId))
        {
            dbContext.Users.Add(new User
            {
                Id = userId,
                Name = "Conversion Owner",
                Email = $"owner-{Guid.NewGuid():N}@example.com",
                PasswordHash = "hash",
                CreatedAt = now,
                IsActive = true
            });
        }

        dbContext.FileAssets.Add(new FileAsset
        {
            Id = sourceFileId,
            UserId = userId,
            OriginalFileName = "document.docx",
            StoredFileName = $"{sourceFileId}.docx",
            StoragePath = $"users/{userId}/originals/{conversionId}/{sourceFileId}.docx",
            BucketName = StorageBuckets.Originals,
            Extension = SupportedFormats.Docx,
            MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            SizeBytes = 100,
            Kind = FileAssetKind.Original,
            CreatedAt = now
        });

        if (outputFileId is not null)
        {
            dbContext.FileAssets.Add(new FileAsset
            {
                Id = outputFileId.Value,
                UserId = userId,
                OriginalFileName = "document.pdf",
                StoredFileName = $"{outputFileId}.pdf",
                StoragePath = $"users/{userId}/converted/{conversionId}/{outputFileId}.pdf",
                BucketName = StorageBuckets.Converted,
                Extension = SupportedFormats.Pdf,
                MimeType = "application/pdf",
                SizeBytes = 4,
                Kind = FileAssetKind.Converted,
                CreatedAt = now,
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddHours(24)
            });
        }

        dbContext.ConversionJobs.Add(new ConversionJob
        {
            Id = conversionId,
            UserId = userId,
            SourceFileId = sourceFileId,
            OutputFileId = outputFileId,
            SourceFormat = SupportedFormats.Docx,
            TargetFormat = SupportedFormats.Pdf,
            Status = status,
            ErrorMessage = status == ConversionStatus.Failed ? "Conversion failed" : null,
            UsageReserved = status != ConversionStatus.Failed,
            CreatedAt = now,
            StartedAt = status == ConversionStatus.Pending ? null : now.AddSeconds(10),
            CompletedAt = status == ConversionStatus.Completed ? now.AddMinutes(1) : null,
            ExpiresAt = outputFileId is null ? null : expiresAt ?? DateTime.UtcNow.AddHours(24)
        });

        await dbContext.SaveChangesAsync();
        return conversionId;
    }
}

internal sealed class NoOpConversionJobQueue : IConversionJobQueue
{
    public void EnqueueConversionJob(Guid conversionJobId)
    {
    }
}

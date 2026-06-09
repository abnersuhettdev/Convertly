using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Convertly.Application.Auth.Dtos;
using Convertly.Application.Common;
using Convertly.Application.Conversions;
using Convertly.Application.Conversions.Dtos;
using Convertly.Application.Files;
using Convertly.Application.Files.Dtos;
using Convertly.Application.Subscriptions;
using Convertly.Domain.Constants;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Convertly.Infrastructure.Storage;
using Convertly.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Convertly.Tests.Conversions;

public sealed class ConversionsApiTests
{
    private const string DocxMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    [Fact]
    public async Task CreateConversion_WithoutToken_ReturnsUnauthorized()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateConversion_WithoutFile_ReturnsError()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(SupportedFormats.Pdf), "targetFormat");

        var response = await client.PostAsync("/api/conversions", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CreateConversionResponse>>();
        Assert.Contains("File is required", body?.Errors ?? []);
    }

    [Fact]
    public async Task CreateConversion_WithEmptyFile_ReturnsError()
    {
        var response = await PostAuthorizedConversionAsync(fileBytes: []);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CreateConversionResponse>>();
        Assert.Contains("File must not be empty", body?.Errors ?? []);
    }

    [Fact]
    public async Task CreateConversion_WithInvalidExtension_ReturnsError()
    {
        var response = await PostAuthorizedConversionAsync(fileName: "document.txt");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CreateConversionResponse>>();
        Assert.Contains("File extension is not supported", body?.Errors ?? []);
    }

    [Fact]
    public async Task CreateConversion_WithInvalidMimeType_ReturnsError()
    {
        var response = await PostAuthorizedConversionAsync(contentType: "text/plain");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CreateConversionResponse>>();
        Assert.Contains("File MIME type is not supported", body?.Errors ?? []);
    }

    [Fact]
    public async Task CreateConversion_WithInvalidTargetFormat_ReturnsError()
    {
        var response = await PostAuthorizedConversionAsync(targetFormat: "docx");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CreateConversionResponse>>();
        Assert.Contains("Target format is not supported", body?.Errors ?? []);
    }

    [Fact]
    public async Task CreateConversion_WhenFileExceedsCurrentPlanSizeLimit_ReturnsError()
    {
        var tooLargeForFreePlan = new byte[(10 * 1024 * 1024) + 1];

        var response = await PostAuthorizedConversionAsync(fileBytes: tooLargeForFreePlan);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CreateConversionResponse>>();
        Assert.Contains("File exceeds current plan size limit", body?.Errors ?? []);
    }

    [Fact]
    public async Task CreateConversion_WithAvailableFreeLimit_CreatesPendingJob()
    {
        var fakeStorage = new FakeFileStorageService();
        var fakeQueue = new FakeConversionJobQueue();
        using var factory = CreateFactory(fakeStorage, fakeQueue);
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent());

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CreateConversionResponse>>();

        Assert.True(body?.Success);
        Assert.NotEqual(Guid.Empty, body!.Data!.ConversionId);
        Assert.Equal(ConversionStatus.Pending.ToString(), body.Data.Status);
        Assert.Equal("Conversion job created", body.Message);
        Assert.Contains(body.Data.ConversionId, fakeQueue.EnqueuedConversionJobIds);
    }

    [Fact]
    public async Task CreateConversion_ReservesOneMonthlyUsage()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent());

        response.EnsureSuccessStatusCode();
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var usage = await dbContext.MonthlyUsages.SingleAsync(usage => usage.UserId == authResponse.User.Id);

        Assert.Equal(1, usage.ConversionsUsed);
    }

    [Fact]
    public async Task CreateConversion_CreatesOriginalFileAsset()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent(fileName: "original.docx"));

        response.EnsureSuccessStatusCode();
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var fileAsset = await dbContext.FileAssets.SingleAsync(asset => asset.UserId == authResponse.User.Id);

        Assert.Equal(FileAssetKind.Original, fileAsset.Kind);
        Assert.Equal("original.docx", fileAsset.OriginalFileName);
        Assert.Equal(StorageBuckets.Originals, fileAsset.BucketName);
        Assert.Equal(SupportedFormats.Docx, fileAsset.Extension);
        Assert.Equal(DocxMimeType, fileAsset.MimeType);
        Assert.Contains($"/originals/", fileAsset.StoragePath);
        Assert.EndsWith(".docx", fileAsset.StoragePath);
    }

    [Fact]
    public async Task CreateConversion_JobReferencesSourceFileId()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent());

        response.EnsureSuccessStatusCode();
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var fileAsset = await dbContext.FileAssets.SingleAsync();
        var conversionJob = await dbContext.ConversionJobs.SingleAsync();

        Assert.Equal(fileAsset.Id, conversionJob.SourceFileId);
        Assert.Null(conversionJob.OutputFileId);
        Assert.True(conversionJob.UsageReserved);
        Assert.Equal(ConversionStatus.Pending, conversionJob.Status);
    }

    [Fact]
    public async Task CreateConversion_WhenMonthlyLimitReached_ReturnsUnprocessableEntity()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        await using var scope = factory.Services.CreateAsyncScope();
        var monthlyUsageService = scope.ServiceProvider.GetRequiredService<IMonthlyUsageService>();
        for (var index = 0; index < 5; index++)
        {
            var reservation = await monthlyUsageService.ReserveConversionAsync(authResponse.User.Id, CancellationToken.None);
            Assert.True(reservation.Success);
        }

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent());

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task CreateConversion_WhenStorageFailsAfterReservation_ReturnsUsage()
    {
        var fakeStorage = new FakeFileStorageService { FailOnSaveOriginal = true };
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        var authResponse = await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var usage = await dbContext.MonthlyUsages.SingleAsync(usage => usage.UserId == authResponse.User.Id);

        Assert.Equal(0, usage.ConversionsUsed);
        Assert.Empty(await dbContext.FileAssets.ToListAsync());
        Assert.Empty(await dbContext.ConversionJobs.ToListAsync());
    }

    [Fact]
    public async Task CreateConversion_ResponseDoesNotExposeSupabaseServiceRoleKey()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        var response = await client.PostAsync("/api/conversions", CreateMultipartContent());
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("development-service-role-key", responseBody);
    }

    private static async Task<HttpResponseMessage> PostAuthorizedConversionAsync(
        byte[]? fileBytes = null,
        string fileName = "document.docx",
        string contentType = DocxMimeType,
        string targetFormat = SupportedFormats.Pdf)
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage);
        using var client = factory.CreateClient();
        await RegisterAndAuthorizeAsync(client);

        return await client.PostAsync(
            "/api/conversions",
            CreateMultipartContent(fileBytes, fileName, contentType, targetFormat));
    }

    private static ConvertlyApiFactory CreateFactory(
        FakeFileStorageService fakeStorage,
        FakeConversionJobQueue? fakeQueue = null)
    {
        fakeQueue ??= new FakeConversionJobQueue();

        return new ConvertlyApiFactory(services =>
        {
            services.RemoveAll<IFileStorageService>();
            services.RemoveAll<IConversionJobQueue>();
            services.AddSingleton<IFileStorageService>(fakeStorage);
            services.AddSingleton<IConversionJobQueue>(fakeQueue);
        });
    }

    private static MultipartFormDataContent CreateMultipartContent(
        byte[]? fileBytes = null,
        string fileName = "document.docx",
        string contentType = DocxMimeType,
        string targetFormat = SupportedFormats.Pdf)
    {
        fileBytes ??= Encoding.UTF8.GetBytes("fake docx content");
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(targetFormat), "targetFormat");

        return content;
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

internal sealed class FakeFileStorageService : IFileStorageService
{
    public bool FailOnSaveOriginal { get; set; }
    public byte[] DownloadBytes { get; set; } = [];
    public List<FileStorageResult> SavedFiles { get; } = [];
    public List<(string BucketName, string StoragePath)> DeletedFiles { get; } = [];

    public Task<FileStorageResult> SaveOriginalAsync(
        Stream file,
        Guid userId,
        Guid conversionId,
        Guid fileId,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (FailOnSaveOriginal)
        {
            throw new InvalidOperationException("Storage failed.");
        }

        var result = new FileStorageResult(
            StorageBuckets.Originals,
            SupabaseStoragePathBuilder.BuildOriginalPath(userId, conversionId, fileId),
            $"{fileId}.docx",
            originalFileName,
            contentType,
            file.CanSeek ? file.Length : 0);

        SavedFiles.Add(result);
        return Task.FromResult(result);
    }

    public Task<FileStorageResult> SaveConvertedAsync(
        Stream file,
        Guid userId,
        Guid conversionId,
        Guid fileId,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var result = new FileStorageResult(
            StorageBuckets.Converted,
            SupabaseStoragePathBuilder.BuildConvertedPath(userId, conversionId, fileId),
            $"{fileId}.pdf",
            fileName,
            contentType,
            file.CanSeek ? file.Length : 0);

        SavedFiles.Add(result);
        return Task.FromResult(result);
    }

    public Task<Stream> GetAsync(string bucketName, string storagePath, CancellationToken cancellationToken)
    {
        return Task.FromResult<Stream>(new MemoryStream(DownloadBytes));
    }

    public Task DeleteAsync(string bucketName, string storagePath, CancellationToken cancellationToken)
    {
        DeletedFiles.Add((bucketName, storagePath));
        return Task.CompletedTask;
    }

    public Task<string> CreateSignedDownloadUrlAsync(
        string bucketName,
        string storagePath,
        TimeSpan expiresIn,
        CancellationToken cancellationToken)
    {
        return Task.FromResult($"https://example.test/{bucketName}/{storagePath}");
    }
}

internal sealed class FakeConversionJobQueue : IConversionJobQueue
{
    public List<Guid> EnqueuedConversionJobIds { get; } = [];

    public void EnqueueConversionJob(Guid conversionJobId)
    {
        EnqueuedConversionJobIds.Add(conversionJobId);
    }
}

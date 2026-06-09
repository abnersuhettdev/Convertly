using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Convertly.Application.Files;
using Convertly.Application.Files.Dtos;
using Microsoft.Extensions.Options;

namespace Convertly.Infrastructure.Storage;

public sealed class SupabaseFileStorageService(
    HttpClient httpClient,
    IOptions<SupabaseStorageOptions> options) : IFileStorageService
{
    private readonly SupabaseStorageOptions _options = options.Value;

    public async Task<FileStorageResult> SaveOriginalAsync(
        Stream file,
        Guid userId,
        Guid conversionId,
        Guid fileId,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var storagePath = SupabaseStoragePathBuilder.BuildOriginalPath(userId, conversionId, fileId);

        return await SaveAsync(
            file,
            _options.OriginalsBucket,
            storagePath,
            originalFileName,
            $"{fileId}.docx",
            contentType,
            cancellationToken);
    }

    public async Task<FileStorageResult> SaveConvertedAsync(
        Stream file,
        Guid userId,
        Guid conversionId,
        Guid fileId,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var storagePath = SupabaseStoragePathBuilder.BuildConvertedPath(userId, conversionId, fileId);

        return await SaveAsync(
            file,
            _options.ConvertedBucket,
            storagePath,
            fileName,
            $"{fileId}.pdf",
            contentType,
            cancellationToken);
    }

    public async Task<Stream> GetAsync(string bucketName, string storagePath, CancellationToken cancellationToken)
    {
        ValidateBucketAndPath(bucketName, storagePath);

        var response = await httpClient.GetAsync(
            $"storage/v1/object/{Uri.EscapeDataString(bucketName)}/{storagePath}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var memoryStream = new MemoryStream();
        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await responseStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task DeleteAsync(string bucketName, string storagePath, CancellationToken cancellationToken)
    {
        ValidateBucketAndPath(bucketName, storagePath);

        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"storage/v1/object/{Uri.EscapeDataString(bucketName)}")
        {
            Content = JsonContent.Create(new DeleteObjectRequest([storagePath]))
        };

        var response = await httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task<string> CreateSignedDownloadUrlAsync(
        string bucketName,
        string storagePath,
        TimeSpan expiresIn,
        CancellationToken cancellationToken)
    {
        ValidateBucketAndPath(bucketName, storagePath);

        if (expiresIn <= TimeSpan.Zero)
        {
            throw new ArgumentException("Signed URL expiration must be greater than zero.", nameof(expiresIn));
        }

        var response = await httpClient.PostAsJsonAsync(
            $"storage/v1/object/sign/{Uri.EscapeDataString(bucketName)}/{storagePath}",
            new SignedUrlRequest((int)expiresIn.TotalSeconds),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var signedUrlResponse = await response.Content.ReadFromJsonAsync<SignedUrlResponse>(cancellationToken);
        if (string.IsNullOrWhiteSpace(signedUrlResponse?.SignedUrl))
        {
            throw new InvalidOperationException("Supabase Storage did not return a signed URL.");
        }

        if (Uri.TryCreate(signedUrlResponse.SignedUrl, UriKind.Absolute, out _))
        {
            return signedUrlResponse.SignedUrl;
        }

        return new Uri(httpClient.BaseAddress!, signedUrlResponse.SignedUrl).ToString();
    }

    private async Task<FileStorageResult> SaveAsync(
        Stream file,
        string bucketName,
        string storagePath,
        string originalFileName,
        string storedFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        ValidateBucketAndPath(bucketName, storagePath);

        if (file.CanSeek)
        {
            file.Position = 0;
        }

        var sizeBytes = file.CanSeek ? file.Length : 0;
        using var content = new StreamContent(file);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"storage/v1/object/{Uri.EscapeDataString(bucketName)}/{storagePath}")
        {
            Content = content
        };
        request.Headers.Add("x-upsert", "true");

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return new FileStorageResult(
            bucketName,
            storagePath,
            storedFileName,
            originalFileName,
            contentType,
            sizeBytes);
    }

    private static void ValidateBucketAndPath(string bucketName, string storagePath)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new ArgumentException("Bucket name is required.", nameof(bucketName));
        }

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException("Storage path is required.", nameof(storagePath));
        }
    }

    private sealed record DeleteObjectRequest(
        [property: JsonPropertyName("prefixes")] IReadOnlyList<string> Prefixes);

    private sealed record SignedUrlRequest(
        [property: JsonPropertyName("expiresIn")] int ExpiresIn);

    private sealed record SignedUrlResponse(
        [property: JsonPropertyName("signedURL")] string? SignedUrl);
}

using Convertly.Application.Files.Dtos;

namespace Convertly.Application.Files;

public interface IFileStorageService
{
    Task<FileStorageResult> SaveOriginalAsync(
        Stream file,
        Guid userId,
        Guid conversionId,
        Guid fileId,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<FileStorageResult> SaveConvertedAsync(
        Stream file,
        Guid userId,
        Guid conversionId,
        Guid fileId,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<Stream> GetAsync(string bucketName, string storagePath, CancellationToken cancellationToken);
    Task DeleteAsync(string bucketName, string storagePath, CancellationToken cancellationToken);
    Task<string> CreateSignedDownloadUrlAsync(
        string bucketName,
        string storagePath,
        TimeSpan expiresIn,
        CancellationToken cancellationToken);
}

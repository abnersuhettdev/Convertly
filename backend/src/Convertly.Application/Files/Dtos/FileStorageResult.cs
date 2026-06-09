namespace Convertly.Application.Files.Dtos;

public sealed record FileStorageResult(
    string BucketName,
    string StoragePath,
    string StoredFileName,
    string OriginalFileName,
    string ContentType,
    long SizeBytes);

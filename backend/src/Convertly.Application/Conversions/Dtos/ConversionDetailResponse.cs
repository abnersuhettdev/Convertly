namespace Convertly.Application.Conversions.Dtos;

public sealed record ConversionDetailResponse(
    Guid Id,
    string SourceFileName,
    string SourceFormat,
    string TargetFormat,
    string Status,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime? ExpiresAt,
    bool DownloadAvailable);

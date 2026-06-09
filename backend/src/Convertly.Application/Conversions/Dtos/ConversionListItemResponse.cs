namespace Convertly.Application.Conversions.Dtos;

public sealed record ConversionListItemResponse(
    Guid Id,
    string SourceFileName,
    string SourceFormat,
    string TargetFormat,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    bool DownloadAvailable);

namespace Convertly.Application.Conversions.Dtos;

public sealed record CreateConversionRequest(
    Stream File,
    string FileName,
    string ContentType,
    long SizeBytes,
    string TargetFormat);

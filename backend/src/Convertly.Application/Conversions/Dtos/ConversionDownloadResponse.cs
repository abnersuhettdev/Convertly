namespace Convertly.Application.Conversions.Dtos;

public sealed record ConversionDownloadResponse(
    Stream File,
    string FileName,
    string ContentType);

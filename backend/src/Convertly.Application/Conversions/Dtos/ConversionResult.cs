namespace Convertly.Application.Conversions.Dtos;

public sealed record ConversionResult(
    string OutputFilePath,
    string FileName,
    string ContentType,
    long SizeBytes);

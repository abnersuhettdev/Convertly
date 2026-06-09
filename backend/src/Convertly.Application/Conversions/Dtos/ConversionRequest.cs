namespace Convertly.Application.Conversions.Dtos;

public sealed record ConversionRequest(
    Guid ConversionJobId,
    Stream SourceFile,
    string SourceFileName,
    string SourceFormat,
    string TargetFormat,
    string WorkingDirectory);

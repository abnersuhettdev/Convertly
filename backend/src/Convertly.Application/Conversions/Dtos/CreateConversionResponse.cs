namespace Convertly.Application.Conversions.Dtos;

public sealed record CreateConversionResponse(
    Guid ConversionId,
    string Status);

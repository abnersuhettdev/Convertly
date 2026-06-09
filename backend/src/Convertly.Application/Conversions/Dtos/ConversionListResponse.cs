namespace Convertly.Application.Conversions.Dtos;

public sealed record ConversionListResponse(
    IReadOnlyList<ConversionListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);

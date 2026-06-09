using Convertly.Application.Common;
using Convertly.Application.Conversions.Dtos;

namespace Convertly.Application.Conversions;

public interface IConversionService
{
    Task<ApiResponse<CreateConversionResponse>> CreateConversionAsync(
        CreateConversionRequest request,
        CancellationToken cancellationToken);

    Task<ApiResponse<ConversionListResponse>> GetConversionsAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken);

    Task<ApiResponse<ConversionDetailResponse>> GetConversionAsync(
        Guid conversionId,
        CancellationToken cancellationToken);

    Task<ApiResponse<ConversionDownloadResponse>> DownloadConversionAsync(
        Guid conversionId,
        CancellationToken cancellationToken);
}

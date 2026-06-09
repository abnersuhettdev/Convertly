using Convertly.Application.Conversions.Dtos;

namespace Convertly.Application.Conversions;

public interface IFileConverter
{
    bool CanConvert(string sourceFormat, string targetFormat);
    Task<ConversionResult> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken);
}

using Convertly.Application.Conversions;

namespace Convertly.Infrastructure.Conversions;

public sealed class FileConverterResolver(IEnumerable<IFileConverter> converters) : IFileConverterResolver
{
    public IFileConverter Resolve(string sourceFormat, string targetFormat)
    {
        return converters.FirstOrDefault(converter => converter.CanConvert(sourceFormat, targetFormat))
            ?? throw new InvalidOperationException($"Conversion from '{sourceFormat}' to '{targetFormat}' is not supported.");
    }
}

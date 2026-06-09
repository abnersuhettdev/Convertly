namespace Convertly.Application.Conversions;

public interface IFileConverterResolver
{
    IFileConverter Resolve(string sourceFormat, string targetFormat);
}

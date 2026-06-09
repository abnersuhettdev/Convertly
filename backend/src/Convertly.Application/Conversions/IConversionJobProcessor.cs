namespace Convertly.Application.Conversions;

public interface IConversionJobProcessor
{
    Task ProcessAsync(Guid conversionJobId);
}

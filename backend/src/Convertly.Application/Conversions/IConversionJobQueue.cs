namespace Convertly.Application.Conversions;

public interface IConversionJobQueue
{
    void EnqueueConversionJob(Guid conversionJobId);
}

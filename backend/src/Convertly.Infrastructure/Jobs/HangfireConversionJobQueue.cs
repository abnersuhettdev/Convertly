using Convertly.Application.Conversions;
using Hangfire;

namespace Convertly.Infrastructure.Jobs;

public sealed class HangfireConversionJobQueue(IBackgroundJobClient backgroundJobClient) : IConversionJobQueue
{
    public void EnqueueConversionJob(Guid conversionJobId)
    {
        backgroundJobClient.Enqueue<IConversionJobProcessor>(
            processor => processor.ProcessAsync(conversionJobId));
    }
}

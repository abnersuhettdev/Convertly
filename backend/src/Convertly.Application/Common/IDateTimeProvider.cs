namespace Convertly.Application.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

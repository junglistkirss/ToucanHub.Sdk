using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Infrastructure;

public interface IEventBus
{
    ValueTask PublishAsync<T>(T e, CancellationToken cancellationToken = default) where T : class, IEvent;
}

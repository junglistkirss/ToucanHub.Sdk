using ToucanHub.Sdk.Infrastructure.Markers;

namespace ToucanHub.Sdk.Infrastructure;

public interface IEventBus
{
    ValueTask PublishAsync<T>(T e, CancellationToken cancellationToken = default) where T : class, IEvent;
}

using ToucanHub.Sdk.Infrastructure.Markers;

namespace ToucanHub.Sdk.Infrastructure.Handlers;

public delegate ValueTask EventHandle<TEvent>(TEvent sourceEvent, CancellationToken ct)
    where TEvent : IEvent;

public interface IEventHandler<TEvent>
    where TEvent : IEvent
{
    ValueTask HandleAsync(TEvent sourceEvent, CancellationToken cancellationToken);
}

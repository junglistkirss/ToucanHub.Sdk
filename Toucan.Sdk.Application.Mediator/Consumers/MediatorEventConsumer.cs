using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Mediator.Consumers;

public class MediatorEventConsumer<T, TEvent, THandler>(ILogger<MediatorEventConsumer<T, TEvent, THandler>> logger, THandler eventHandler) : IMediatorConsumer<T>
    where T : class, TEvent, IEvent
    where TEvent : class, IEvent
    where THandler : class, IEventHandler<TEvent>
{
    public virtual async ValueTask Consume(MediatorContext<T> context)
    {
        logger.LogDebug("BEGIN consume event {eventType}", typeof(T));
        await eventHandler.HandleAsync(context.Message, context.CancellationToken).ConfigureAwait(false);
        logger.LogDebug("END consume event {eventType}", typeof(T));
    }
}


public sealed class MediatorEventHandleConsumer<T, TEvent>(ILogger<MediatorEventHandleConsumer<T, TEvent>> logger, EventHandle<TEvent> eventHandler) : IMediatorConsumer<T>
     where T : class, TEvent, IEvent
    where TEvent : class, IEvent
{
    public async ValueTask Consume(MediatorContext<T> context)
    {
        logger.LogDebug("BEGIN consume {eventType}", typeof(T));
        await eventHandler(context.Message, context.CancellationToken).ConfigureAwait(false);
        logger.LogDebug("END consume event {eventType}", typeof(T));
    }
}

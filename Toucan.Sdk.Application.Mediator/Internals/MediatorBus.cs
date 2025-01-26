using Toucan.Sdk.Application.Mediator.Consumers;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Mediator.Internals;

internal sealed class MediatorBus(IServiceProvider provider) : IMediatorBus
{
    public async ValueTask SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class, ICommand
    {
        IMediatorConsumer<T> consumer = provider.GetRequiredService<IMediatorConsumer<T>>();
        MediatorContext<T> context = new()
        {
            Message = command,
            CancellationToken = cancellationToken
        };
        await consumer.Consume(context).ConfigureAwait(false);
    }

    public ValueTask<TResponse> SendAsync<T, TResponse>(T command, CancellationToken cancellationToken = default)
        where T : class, ICommand
        where TResponse : class
    {
        IMediatorConsumer<T, TResponse> consumer = provider.GetRequiredService<IMediatorConsumer<T, TResponse>>();
        MediatorContext<T> context = new()
        {
            Message = command,
            CancellationToken = cancellationToken
        };
        return consumer.Consume(context);
    }

    public async ValueTask PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : class, IEvent
    {
        IEnumerable<IMediatorConsumer<T>> consumers = provider.GetServices<IMediatorConsumer<T>>();
        MediatorContext<T> context = new()
        {
            Message = @event,
            CancellationToken = cancellationToken
        };
        foreach (IMediatorConsumer<T> consumer in consumers)
        {
            await consumer.Consume(context).ConfigureAwait(false);
        }
    }

    public ValueTask<TResponse> QueryAsync<T, TResponse>(T query, CancellationToken cancellationToken = default)
        where T : class, IQuery
        where TResponse : class
    {
        IMediatorConsumer<T, TResponse> consumer = provider.GetRequiredService<IMediatorConsumer<T, TResponse>>();
        MediatorContext<T> context = new()
        {
            Message = query,
            CancellationToken = cancellationToken
        };
        return consumer.Consume(context);

    }
}
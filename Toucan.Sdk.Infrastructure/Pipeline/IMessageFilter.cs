namespace Toucan.Infrastructure.Pipeline;

public interface IMessageFilter<in TMessage> : IMessageMiddleware
{
    Type IMessageMiddleware.CanHandle => typeof(TMessage);

    async ValueTask<object?> IMessageMiddleware.Handle(object message, CancellationToken ct)
    {
        bool matches = await Handle((TMessage)message, ct);

        return matches ? message : null;
    }

    ValueTask<bool> Handle(TMessage message, CancellationToken ct);
}

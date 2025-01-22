using Toucan.Infrastructure.Handlers;

namespace Toucan.Infrastructure.Pipeline;

public interface IMessageTransformation<in TMessage, TTransformedMessage> : IMessageMiddleware<TMessage>
{
    Type IMessageMiddleware.CanHandle => typeof(TMessage);

    async ValueTask<object?> IMessageMiddleware.Handle(object message, CancellationToken ct)
    {
        return await Handle((TMessage)message, ct);
    }

    new ValueTask<TTransformedMessage> Handle(TMessage message, CancellationToken ct);
}

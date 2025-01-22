using Toucan.Infrastructure.Markers;

namespace Toucan.Infrastructure.Pipeline;

public class MessageHandlerWrapper<TMessage> : IMessageMiddleware<TMessage>
{
    private readonly Func<TMessage, CancellationToken, ValueTask> handler;

    public MessageHandlerWrapper(Func<TMessage, CancellationToken, ValueTask> handler)
    {
        this.handler = handler;
    }

    public MessageHandlerWrapper(Action<TMessage> handler)
    {
        this.handler = (message, _) =>
        {
            handler(message);
            return ValueTask.CompletedTask;
        };
    }

    public ValueTask Handle(TMessage message, CancellationToken ct) =>
        handler(message, ct);
}

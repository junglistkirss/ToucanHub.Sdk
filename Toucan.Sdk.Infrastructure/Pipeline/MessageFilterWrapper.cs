namespace Toucan.Infrastructure.Pipeline;

public class MessageFilterWrapper<TMessage> : IMessageFilter<TMessage>
{
    private readonly Func<TMessage, CancellationToken, ValueTask<bool>> handler;

    public MessageFilterWrapper(Func<TMessage, CancellationToken, ValueTask<bool>> handler)
    {
        this.handler = handler;
    }

    public MessageFilterWrapper(Func<TMessage, bool> handler)
    {
        this.handler = (message, _) =>
        {
            bool result = handler(message);
            return ValueTask.FromResult(result);
        };
    }

    public ValueTask<bool> Handle(TMessage message, CancellationToken ct) =>
        handler(message, ct);
}
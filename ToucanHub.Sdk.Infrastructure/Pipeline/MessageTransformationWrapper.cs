namespace Toucan.Infrastructure.Pipeline;

public class MessageTransformationWrapper<TMessage, TTransformedMessage> : IMessageTransformation<TMessage, TTransformedMessage>
{
    private readonly Func<TMessage, CancellationToken, ValueTask<TTransformedMessage>> handler;

    public MessageTransformationWrapper(Func<TMessage, CancellationToken, ValueTask<TTransformedMessage>> handler)
    {
        this.handler = handler;
    }

    public MessageTransformationWrapper(Func<TMessage, TTransformedMessage> handler)
    {
        this.handler = (message, _) =>
        {
            TTransformedMessage? result = handler(message);
            return ValueTask.FromResult(result);
        };
    }

    public  ValueTask<TTransformedMessage> Handle(TMessage message, CancellationToken ct) =>
        handler(message, ct);

    ValueTask IMessageMiddleware<TMessage>.Handle(TMessage message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

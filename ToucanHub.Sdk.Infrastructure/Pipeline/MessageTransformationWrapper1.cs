namespace Toucan.Infrastructure.Pipeline;

public class MessageTransformationWrapper<TMessage> : MessageTransformationWrapper<TMessage, object>
{
    public MessageTransformationWrapper(Func<TMessage, CancellationToken, ValueTask<object>> handler) : base(handler)
    {
    }

    public MessageTransformationWrapper(Func<TMessage, object> handler) : base(handler)
    {
    }
}

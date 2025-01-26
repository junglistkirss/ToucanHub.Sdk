using Toucan.Sdk.Contracts.Messages.Envelopes;

namespace Toucan.Sdk.Shared.Messages;


public static class Envelope
{
    private delegate TDestination EnvelopeWrapper<TDestination, TMessage>(TDestination dest, TMessage msg)
        where TDestination : IEnvelope, new()
        where TMessage : IMessage;

    private static TDestination Wrap<TDestination, TMessage>(TMessage message, MessageHeaders metadatas, EnvelopeWrapper<TDestination, TMessage> initalizer)
        where TDestination : EnvelopeMessage, new()
        where TMessage : IMessage
    {
        TDestination destination = new()
        {
            Headers = metadatas,
        };
        return initalizer(destination, message);
    }

    public static EnvelopeMessage WrapMessage<TMessage>(this TMessage message, MessageHeaders? metadatas = null)
        where TMessage : IMessage
    {
        return Wrap<EnvelopeMessage, TMessage>(
            message,
            metadatas ?? MessageHeaders.Empty,
            (dest, msg) => dest with
            {
                Message = msg,
            });
    }
    public static EventEnvelope<TMessage> WrapEvent<TMessage>(this TMessage message, MessageHeaders? metadatas = null)
        where TMessage : EventMessage
    {
        return Wrap<EventEnvelope<TMessage>, TMessage>(
            message,
            metadatas ?? MessageHeaders.Empty,
            (dest, msg) => dest with
            {
                Message = msg,
            });
    }

    public static CommandEnvelope<TMessage> WrapCommand<TMessage>(this TMessage message, MessageHeaders? metadatas = null)
        where TMessage : CommandMessage
    {
        return Wrap<CommandEnvelope<TMessage>, TMessage>(
            message,
            metadatas ?? MessageHeaders.Empty,
            (dest, msg) => dest with
            {
                Message = msg,
            });
    }

    public static QueryEnvelope<TMessage> WrapQuery<TMessage>(this TMessage message, MessageHeaders? metadatas = null)
        where TMessage : QueryMessage
    {
        return Wrap<QueryEnvelope<TMessage>, TMessage>(
            message,
            metadatas ?? MessageHeaders.Empty,
            (dest, msg) => dest with
            {
                Message = msg,
            });
    }

    public static IEnumerable<EnvelopeMessage> WrapMessages<TMessage>(this IEnumerable<TMessage> messages, MessageHeaders? metadatas = null)
        where TMessage : IMessage
        => messages.Select(m => m.WrapMessage(metadatas));

    public static bool UnwrapTo<T>(this IMessage message, [NotNullWhen(true)] out T? outputMessage)
         where T : IMessage
    {
        outputMessage = default;
        switch (message)
        {
            case IEnvelope envelope:
                IMessage msg = envelope.UnwrapMessage();
                if (msg is T m)
                {
                    outputMessage = m;
                    return true;
                }
                break;
            case T inner:
                outputMessage = inner;
                return true;
            default:

                break;
        }
        return false;
    }

    public static IMessage UnwrapMessage(this IEnvelope message)
    {
        if (message is EnvelopeMessage envelopeMessage)
            return envelopeMessage.Message;
        return new EmptyMessage();
    }

    public static bool Of<T>(this IEnvelope envelope)
       where T : IMessage
    {
        if (envelope.UnwrapMessage() is T)
        {
            return true;
        }
        return false;
    }

    public static bool Unwrap<T>(this IEnvelope envelope, [NotNullWhen(true)] out T? message)
        where T : IMessage
    {
        if (envelope.UnwrapMessage() is T typedMessage)
        {
            message = typedMessage;
            return true;
        }
        message = default;
        return false;
    }
}

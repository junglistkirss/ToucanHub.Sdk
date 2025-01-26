using Toucan.Sdk.Application.Context;
using Toucan.Sdk.Application.Exceptions;
using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Contracts.Wrapper;
using Toucan.Sdk.Infrastructure.Markers;
using Toucan.Sdk.Shared.Messages;

namespace Toucan.Sdk.Application.Services;

public abstract class BaseDispatcher(IContext context) : ICommandSender, IRequestSender, IPublisher
{


    public IContext Context { get; } = context;

    protected virtual MessageHeaders GetMessageMetadata()
    {
        return MessageHeaders.Create(DateTimeOffset.UtcNow, Context.User.Issuer, Context.Origin, [.. Context.Metadatas]);
    }

    public ValueTask PublishEventMessage<T>(T message) where T : EventMessage
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message);
            EventEnvelope<T> ev = message.WrapEvent(metadatas: GetMessageMetadata());
            return PublishEvent(ev);
        }
        catch (Exception e)
        {
            throw new DispatchException("Publish failed", e)
            {
                ObjectSource = message
            };
        }
    }

    public ValueTask SendCommandMessage<T>(T message) where T : CommandMessage
    {
        try
        {
            CommandEnvelope<T> ev = message.WrapCommand(metadatas: GetMessageMetadata());
            return SendCommand(ev);
        }
        catch (Exception e)
        {
            throw new DispatchException("Send failed", e)
            {
                ObjectSource = message
            };
        }
    }

    public ValueTask<TResponse> SendQueryMessage<T, TResponse>(T message)
        where T : QueryMessage<TResponse>
        where TResponse : Result
    {
        try
        {
            QueryEnvelope<T> query = message.WrapQuery(metadatas: GetMessageMetadata());
            return SendQuery<QueryEnvelope<T>, TResponse>(query);

        }
        catch (Exception e)
        {
            throw new DispatchException("Request failed", e)
            {
                ObjectSource = message
            };
        }
    }

    public ValueTask<TResponse> SendCommandMessageWait<T, TResponse>(T message)
        where T : CommandMessage
        where TResponse : Result
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message);
            CommandEnvelope<T> ev = message.WrapCommand(metadatas: GetMessageMetadata());
            return SendCommandWait<CommandEnvelope<T>, TResponse>(ev);
        }
        catch (Exception e)
        {
            throw new DispatchException("Send failed", e)
            {
                ObjectSource = message
            };
        }
    }

    public abstract ValueTask PublishEvent<T>(T message)
        where T : class, IEvent;
    public abstract ValueTask SendCommand<T>(T message)
        where T : class, ICommand;
    public abstract ValueTask<TResponse> SendCommandWait<T, TResponse>(T message)
        where T : class, ICommand
        where TResponse : Result;
    public abstract ValueTask<TResponse> SendQuery<T, TResponse>(T message)
        where T : class, IQuery
        where TResponse : Result;

}

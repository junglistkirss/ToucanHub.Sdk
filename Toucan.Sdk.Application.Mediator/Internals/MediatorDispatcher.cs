using Toucan.Sdk.Application.Context;
using Toucan.Sdk.Application.Exceptions;
using Toucan.Sdk.Application.Services;

namespace Toucan.Sdk.Application.Mediator.Internals;

internal sealed class MediatorDispatcher(IMediatorBus bus, IContext context) : BaseDispatcher(context), IMediatorCommandSender, IMediatorRequestSender, IMediatorPublisher
{
    public override async ValueTask SendCommand<T>(T message)
    {
        try
        {
            await bus.SendAsync(message, Context.CancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new DispatchException("Send failed", e)
            {
                ObjectSource = message
            };
        }
    }

    public override async ValueTask<TResponse> SendCommandWait<T, TResponse>(T message)
    {
        try
        {
            return await bus.SendAsync<T, TResponse>(message, Context.CancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new DispatchException("Send failed", e)
            {
                ObjectSource = message
            };
        }
    }

    public override async ValueTask<TResponse> SendQuery<T, TResponse>(T message)
    {
        try
        {
            return await bus.QueryAsync<T, TResponse>(message, Context.CancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new DispatchException("Request failed", e)
            {
                ObjectSource = message
            };
        }
    }

    public override async ValueTask PublishEvent<T>(T message)
    {
        try
        {
            await bus.PublishAsync(message, Context.CancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new DispatchException("Publish failed", e)
            {
                ObjectSource = message
            };
        }
    }
}
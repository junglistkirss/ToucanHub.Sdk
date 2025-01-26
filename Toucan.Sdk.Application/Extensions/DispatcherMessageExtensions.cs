using Toucan.Sdk.Application.Exceptions;
using Toucan.Sdk.Application.Services;
using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Application.Extensions;
public static class DispatcherMessageExtensions
{
    public static async ValueTask<Result> TrySend<T>(this ICommandSender commandSender, T message, Func<T, DispatchException, Result>? defaultResponse = null)
        where T : CommandMessage
    {
        try
        {
            await commandSender.SendCommandMessage(message).ConfigureAwait(false);
            return Result.Success();
        }
        catch (DispatchException de)
        {
            if (defaultResponse != null)
                return defaultResponse.Invoke(message, de);
            throw;
        }
    }

    public static async ValueTask<TResponse> TrySendWait<T, TResponse>(this ICommandSender commandSender, T message, Func<T, DispatchException, TResponse>? defaultResponse = null)
        where T : CommandMessage
        where TResponse : Result
    {
        try
        {
            TResponse response = await commandSender.SendCommandMessageWait<T, TResponse>(message).ConfigureAwait(false);
            return response;
        }
        catch (DispatchException de)
        {
            if (defaultResponse != null)
                return defaultResponse.Invoke(message, de);
            throw;
        }

    }

    public static async ValueTask<Result> TryPublish<T>(this IPublisher eventPublisher, T message, Func<T, DispatchException, Result>? defaultResponse = null)
        where T : EventMessage
    {
        try
        {
            await eventPublisher.PublishEventMessage(message).ConfigureAwait(false);
            return Result.Success();
        }
        catch (DispatchException de)
        {
            if (defaultResponse != null)
                return defaultResponse.Invoke(message, de);
            throw;
        }

    }

    public static async ValueTask<TResponse> TryRequest<T, TResponse>(this IRequestSender requestSender, T message, Func<T, DispatchException, TResponse>? defaultResponse = null)
       where T : QueryMessage<TResponse>
        where TResponse : Result
    {
        try
        {
            TResponse resp = await requestSender.SendQueryMessage<T, TResponse>(message).ConfigureAwait(false);
            return resp;
        }
        catch (DispatchException de)
        {
            if (defaultResponse != null)
                return defaultResponse.Invoke(message, de);
            throw;
        }
    }






}


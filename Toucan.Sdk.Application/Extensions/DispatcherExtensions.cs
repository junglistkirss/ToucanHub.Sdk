using Toucan.Sdk.Application.Exceptions;
using Toucan.Sdk.Application.Services;
using Toucan.Sdk.Contracts.Wrapper;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Extensions;
public static class DispatcherExtensions
{
    public static async ValueTask TrySend<T>(this ICommandSender commandSender, T message, Action<T, DispatchException>? errorFallback = null)
        where T : class, ICommand
    {
        try
        {
            await commandSender.SendCommand(message).ConfigureAwait(false);
        }
        catch (DispatchException de)
        {
            errorFallback?.Invoke(message, de);
            throw;
        }
    }

    public static async ValueTask<TResponse> TrySendWait<T, TResponse>(this ICommandSender commandSender, T message, Action<T, DispatchException>? errorFallback = null)
        where T : class, ICommand
        where TResponse : Result
    {
        try
        {
            TResponse response = await commandSender.SendCommandWait<T, TResponse>(message).ConfigureAwait(true);
            return response;
        }
        catch (DispatchException de)
        {
            errorFallback?.Invoke(message, de);
            throw;
        }

    }




    public static async ValueTask TryPublish<T>(this IPublisher eventPublisher, T message, Action<T, DispatchException>? errorFallback = null)
        where T : class, IEvent
    {
        try
        {
            await eventPublisher.PublishEvent(message).ConfigureAwait(false);
        }
        catch (DispatchException de)
        {
            errorFallback?.Invoke(message, de);
            throw;
        }

    }
    public static async ValueTask<TResponse> TryRequest<T, TResponse>(this IRequestSender requestSender, T message, Action<T, DispatchException>? errorFallback = null)
       where T : class, IQuery
        where TResponse : Result
    {
        try
        {
            TResponse resp = await requestSender.SendQuery<T, TResponse>(message).ConfigureAwait(true);
            return resp;
        }
        catch (DispatchException de)
        {
            errorFallback?.Invoke(message, de);
            throw;
        }
    }






}


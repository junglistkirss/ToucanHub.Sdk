using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Mediator.Consumers;

public class MediatorQueryResponseConsumer<T, TQuery, THandler, TResponse>(ILogger<MediatorQueryResponseConsumer<T, TQuery, THandler, TResponse>> logger, THandler queryHandler) : IMediatorConsumer<T, TResponse>
    where T : class, TQuery, IQuery
    where TQuery : class, IQuery
    where TResponse : class
    where THandler : class, IQueryHandler<TQuery, TResponse>
{
    public virtual async ValueTask<TResponse> Consume(MediatorContext<T> context)
    {

        logger.LogDebug($"BEGIN consume query {typeof(T)}");
        TResponse response = await queryHandler.RequestAsync(context.Message, context.CancellationToken);
        logger.LogDebug($"END consume query {typeof(T)}");
        logger.LogDebug($"BEGIN respond {typeof(TResponse)}");
        return response;
    }
}

public sealed class MediatorQueryResponseHandleConsumer<T, TQuery, TResponse>(ILogger<MediatorQueryResponseHandleConsumer<T, TQuery, TResponse>> logger, QueryHandle<TQuery, TResponse> queryHandler) : IMediatorConsumer<T, TResponse>
    where T : class, TQuery, IQuery
    where TQuery : class, IQuery
    where TResponse : class
{
    public async ValueTask<TResponse> Consume(MediatorContext<T> context)
    {

        logger.LogDebug($"BEGIN consume query {typeof(T)}");
        TResponse response = await queryHandler(context.Message, context.CancellationToken);
        logger.LogDebug($"END consume query {typeof(T)}");
        logger.LogDebug($"BEGIN respond {typeof(TResponse)}");
        return response;
    }
}
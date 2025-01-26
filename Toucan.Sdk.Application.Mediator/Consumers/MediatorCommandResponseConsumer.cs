using System.Diagnostics;
using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Mediator.Consumers;

public class MediatorCommandResponseConsumer<T, TCommand, TResponse, THandler>(ILogger<MediatorCommandResponseConsumer<T, TCommand, TResponse, THandler>> logger, THandler commandHandler) : IMediatorConsumer<T, TResponse>
    where T : class, TCommand, ICommand
    where TCommand : class, ICommand
    where TResponse : class
    where THandler : ICommandHandler<TCommand, TResponse>
{
    public virtual async ValueTask<TResponse> Consume(MediatorContext<T> context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Consume command {type} starts", typeof(T));
        TResponse response = await commandHandler.HandleAsync(context.Message, context.CancellationToken).ConfigureAwait(false);
        logger.LogInformation("Command handled in {elapsed}", stopwatch.Elapsed);
        return response;
    }
}

public sealed class MediatorCommandResponseHandleConsumer<T, TCommand, TReponse>(ILogger<MediatorCommandResponseHandleConsumer<T, TCommand, TReponse>> logger, CommandHandle<TCommand, TReponse> commandHandler) : IMediatorConsumer<T, TReponse>
    where T : class, TCommand, ICommand
    where TCommand : class, ICommand
    where TReponse : class
{
    public async ValueTask<TReponse> Consume(MediatorContext<T> context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Consume command {type} starts", typeof(T));
        TReponse response = await commandHandler(context.Message, context.CancellationToken).ConfigureAwait(false);
        logger.LogInformation("Command handled in {elapsed}", stopwatch.Elapsed);
        return response;

    }
}

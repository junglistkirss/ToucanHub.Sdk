using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Mediator.Consumers;

public class MediatorCommandConsumer<T, TCommand, THandler>(ILogger<MediatorCommandConsumer<T, TCommand, THandler>> logger, THandler commandHandler) : IMediatorConsumer<T>
    where T : class, TCommand, ICommand
    where TCommand : class, ICommand
    where THandler : class, ICommandHandler<TCommand>
{
    public virtual async ValueTask Consume(MediatorContext<T> context)
    {
        logger.LogDebug("BEGIN consume command {type}", typeof(T));
        await commandHandler.HandleAsync(context.Message, context.CancellationToken).ConfigureAwait(false);
        logger.LogDebug("END consume command {type}", typeof(T));
    }
}

public sealed class MediatorCommandHandleConsumer<T, TCommand>(ILogger<MediatorCommandHandleConsumer<T, TCommand>> logger, CommandHandle<TCommand> commandHandle) : IMediatorConsumer<T>
    where T : class, TCommand, ICommand
    where TCommand : class, ICommand
{
    public async ValueTask Consume(MediatorContext<T> context)
    {
        logger.LogDebug("BEGIN consume command {type}", typeof(T));
        await commandHandle(context.Message, context.CancellationToken).ConfigureAwait(false);
        logger.LogDebug("END consume command {type}", typeof(T));
    }
}

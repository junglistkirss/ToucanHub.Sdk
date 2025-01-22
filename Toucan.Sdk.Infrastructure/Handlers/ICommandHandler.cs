using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Infrastructure.Handlers;

public delegate ValueTask CommandHandle<TCommand>(TCommand command, CancellationToken ct)
    where TCommand : ICommand;


public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    ValueTask HandleAsync(TCommand command, CancellationToken ct);
}

public delegate Task<TResponse> CommandHandle<TCommand, TResponse>(TCommand command, CancellationToken ct)
    where TCommand : ICommand
    where TResponse : class;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
    where TResponse : class
{
    ValueTask<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Infrastructure;

public interface ICommandBus
{
    ValueTask SendAsync<T>(T c, CancellationToken cancellationToken = default) where T : class, ICommand;
    ValueTask<TResponse> SendAsync<T, TResponse>(T c, CancellationToken cancellationToken = default)
        where T : class, ICommand
        where TResponse : class;
}

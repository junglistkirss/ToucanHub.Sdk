using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Infrastructure;

public interface IQueryBus
{
    ValueTask<TResponse> QueryAsync<T, TResponse>(T q, CancellationToken cancellationToken = default)
        where T : class, IQuery
        where TResponse : class;
}

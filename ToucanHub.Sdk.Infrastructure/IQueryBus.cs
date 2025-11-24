using ToucanHub.Sdk.Infrastructure.Markers;

namespace ToucanHub.Sdk.Infrastructure;

public interface IQueryBus
{
    ValueTask<TResponse> QueryAsync<T, TResponse>(T q, CancellationToken cancellationToken = default)
        where T : class, IQuery
        where TResponse : class;
}

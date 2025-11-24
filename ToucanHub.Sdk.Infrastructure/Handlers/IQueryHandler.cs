using ToucanHub.Sdk.Infrastructure.Markers;

namespace ToucanHub.Sdk.Infrastructure.Handlers;

public delegate ValueTask<TResponse> QueryHandle<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
    where TQuery : IQuery
    where TResponse : class;

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery
    where TResponse : class
{
    ValueTask<TResponse> RequestAsync(TQuery query, CancellationToken cancellationToken = default);
}

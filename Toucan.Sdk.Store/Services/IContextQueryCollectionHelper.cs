using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Store.Extensions;

namespace Toucan.Sdk.Store.Services;


public interface IContextQueryCollectionHelper<TContext> : IContextWrapper<TContext>
    where TContext : IReadContextProxy
{
    ValueTask<PartialCollection<TEntity>?> FetchCollectionAsync<TEntity, TFilter, TSearch, TSort>(
        EntityQuerySelector<TContext, TEntity> selector,
        ICollectionQuery<TFilter, TSearch, TSort>? query,
        CancellationToken cancellationToken = default
    )
        where TFilter : BaseFilterNode<TSearch>
        where TEntity : class
        where TSort : notnull;

    ValueTask<PartialCollection<TEntity>?> FetchCollectionAsync<TEntity, TSortEnum>(
        EntityQuerySelector<TContext, TEntity> selector,
        IPaginatedSet<TSortEnum>? page,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
        where TSortEnum : notnull;
}

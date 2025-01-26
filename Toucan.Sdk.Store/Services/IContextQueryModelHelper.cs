using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Store.Extensions;

namespace Toucan.Sdk.Store.Services;

public interface IContextQueryModelHelper<TContext> : IContextWrapper<TContext>
    where TContext : IReadContextProxy
{
    ValueTask<TEntity?> FetchModelAsync<TEntity, TFilter, TSearch>(
        EntityQuerySelector<TContext, TEntity> selector,
        IModelQuery<TFilter, TSearch>? query,
        CancellationToken cancellationToken = default
    ) where TFilter : BaseFilterNode<TSearch>
        where TEntity : class;

}

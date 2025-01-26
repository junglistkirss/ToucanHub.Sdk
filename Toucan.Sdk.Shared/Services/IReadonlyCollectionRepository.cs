using Toucan.Sdk.Contracts.Entities;
using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Shared.Services;

public interface IReadonlyCollectionRepository<T, TId>
    where TId : struct
    where T : class, IEntity<TId>
{
    ValueTask<Results<T>> Browse<TSort>(IPaginatedSet<TSort> page, CancellationToken cancellationToken)
        where TSort : notnull;
    ValueTask<Results<T>> Search<TFilter, TSearch, TSort>(ICollectionQuery<TFilter, TSearch, TSort> msg, CancellationToken cancellationToken)
        where TFilter : BaseFilterNode<TSearch>
        where TSort : notnull;
}

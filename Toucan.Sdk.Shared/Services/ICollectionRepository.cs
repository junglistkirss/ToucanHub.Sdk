using Toucan.Sdk.Contracts.Entities;
using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Shared.Services;


public interface ICollectionRepository<T, TId>
    where TId : struct
    where T : class,  IEntity<TId>
{
    Task<Result> Insert(T obj, CancellationToken cancellationToken);
    Task<Result> Update(T obj, CancellationToken cancellationToken);

    Task<Result> Upsert(T obj, CancellationToken cancellationToken);
    //Task<Result> Patch(Func<T, T> obj, CancellationToken cancellationToken);

    Task<Result> Delete(T obj, CancellationToken cancellationToken);
    Task<Result> Delete(TId key, CancellationToken cancellationToken);
}

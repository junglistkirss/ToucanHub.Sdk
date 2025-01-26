using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Toucan.Sdk.Store.Services;


public interface IContextModelPersistence<TContext> : IContextWrapper<TContext>
    where TContext : IWriteContextProxy
{
    ValueTask PersistAsync<TEntity>(
        TEntity src,
        Action<EntityEntry<TEntity>> action,
        CancellationToken cancellationToken = default
    ) where TEntity : class;
}

//public interface IModelPersistenceManager<TContext>
//{
//    Task PersistModelAsync<TEntity>(
//        TEntity? model,
//        CancellationToken cancellationToken = default
//    );
//}

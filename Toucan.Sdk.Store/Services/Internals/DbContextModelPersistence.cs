using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Toucan.Sdk.Store.Services.Internals;

internal class DbContextModelPersistence<TProxy, TContext>(TContext context, IServiceProvider serviceProvider, ILogger<DbContextModelPersistence<TProxy, TContext>> logger) : BaseDbContextHelper<TProxy, TContext>(context, serviceProvider, logger), IContextModelPersistence<TProxy>
    where TProxy : IWriteContextProxy
    where TContext : DbContext, TProxy
{
    public async ValueTask PersistAsync<TEntity>(TEntity model, Action<EntityEntry<TEntity>> action, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        try
        {
            if (Context is TContext contextProxy)
            {
                EntityEntry<TEntity> modelEntry = contextProxy.Entry(model);
                foreach (NavigationEntry nav in modelEntry.Navigations)
                {
                    if (nav.Metadata.TargetEntityType.IsOwned() && !nav.IsLoaded)
                        await nav.LoadAsync(LoadOptions.ForceIdentityResolution, cancellationToken);
                }
                await modelEntry.ReloadAsync(cancellationToken);

                if (modelEntry.State == EntityState.Detached)
                    modelEntry.State = EntityState.Added;

                action(modelEntry);

                await contextProxy.SaveChangesAsync(cancellationToken);
                //if (modelEntry.IsKeySet)
                //{
                //    EntityState state = await UpdateEntry(modelEntry, cancellationToken);
                //    if (modelEntry.State != state)
                //        modelEntry.State = state;

                //    foreach (ReferenceEntry reference in modelEntry.References)
                //        if (reference.Metadata.TargetEntityType.IsOwned() && reference.IsLoaded)
                //        {

                //        }

                //    foreach (CollectionEntry collection in modelEntry.Collections)
                //        if (collection.Metadata.TargetEntityType.IsOwned() && collection.IsLoaded)
                //        {
                //            IEnumerable? currentCollection = collection.CurrentValue;
                //            if (currentCollection is not null)
                //                foreach (object item in currentCollection)
                //                {
                //                    EntityEntry? itemEntry = collection.FindEntry(item);
                //                    EntityState itemState = await UpdateEntry(itemEntry, cancellationToken);
                //                    if (itemEntry.State != itemState)
                //                        itemEntry.State = itemState;
                //                }


                //        }

                //}
                //else
                //{
                //    modelEntry.State = EntityState.Added;
                //}
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("PersistenceError", ex);
        }
    }

}

//public interface IModelPersistenceManager<TContext>
//{
//    Task PersistModelAsync<TEntity>(
//        TEntity? model,
//        CancellationToken cancellationToken = default
//    );
//}

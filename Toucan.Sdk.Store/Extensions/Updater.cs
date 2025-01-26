using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Toucan.Sdk.Store.Extensions;

public static class Updater
{
    public static void OnState<TEntity>(this EntityEntry<TEntity> entityEntry, EntityState state, Action<EntityEntry<TEntity>> action)
        where TEntity : class
    {
        if (entityEntry.State == state)
            action(entityEntry);
    }
    public static void MarkDeleted<TEntity>(this EntityEntry<TEntity> entityEntry)
        where TEntity : class
    {
        entityEntry.State = EntityState.Deleted;
    }

    public static void Set<TEntity, TProperty>(this PropertyEntry<TEntity, TProperty> propertyEntry, Func<PropertyEntry<TEntity, TProperty>, TProperty, TProperty> propertyValue)
        where TEntity : class
    {
        ValueComparer comparer = propertyEntry.Metadata.GetValueComparer();
        TProperty current = propertyEntry.OriginalValue;
        if (!comparer.Equals(current, propertyValue))
        {
            propertyEntry.CurrentValue = propertyValue(propertyEntry, current);
            propertyEntry.IsModified = true;
        }
    }

    public static void Set<TEntity, TProperty>(this PropertyEntry<TEntity, TProperty> propertyEntry, TProperty propertyValue)
        where TEntity : class
    {
        ValueComparer comparer = propertyEntry.Metadata.GetValueComparer();
        TProperty current = propertyEntry.OriginalValue;
        if (!comparer.Equals(current, propertyValue))
        {
            propertyEntry.CurrentValue = propertyValue;
            propertyEntry.IsModified = true;
        }
    }
    //public static void AddItemBy<TEntity, TItem, TKey>(this CollectionEntry<TEntity, TItem> collectionEntry, Func<TItem, TKey> keySelector, TItem itemCollection, IEqualityComparer<TKey>? comparer = null)
    //    where TEntity : class
    //    where TItem : class
    //    where TKey : notnull
    //{
    //    collectionEntry.MergeBy(keySelector, [itemCollection], (_, _) => { }, comparer);
    //}

    public static void UpdateSingleItemBy<TEntity, TItem, TKey>(
        this CollectionEntry<TEntity, TItem> collectionEntry,
        Func<TItem, TKey> keySelector,
        TKey key,
        Action<EntityEntry<TItem>> itemUpdate)
    where TEntity : class
    where TItem : class
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(keySelector);

        // If the collection is null or empty, we can't update anything
        if (collectionEntry.CurrentValue == null || !collectionEntry.CurrentValue.Any())
        {
            throw new InvalidOperationException("Cannot update item in an empty or null collection.");
        }

        // Find the existing item in the collection
        TItem? existingItem = collectionEntry.CurrentValue.FirstOrDefault(x => keySelector(x).Equals(key))
            ?? throw new InvalidOperationException("Item to update was not found in the collection.");

        EntityEntry<TItem> entry = collectionEntry.FindEntry(existingItem)
            ?? throw new InvalidOperationException("Item to update was not found in the collection.");

        if (entry.State == EntityState.Detached)
            entry.State = EntityState.Unchanged;

        itemUpdate(entry);
    }

    public static void UpdateMany<TEntity, TItem>(this CollectionEntry<TEntity, TItem> collectionEntry, Action<EntityEntry<TItem>> updater)
        where TEntity : class
        where TItem : class
    {
        if (collectionEntry.CurrentValue != null)
            foreach (TItem item in collectionEntry.CurrentValue)
            {
                EntityEntry<TItem>? entry = collectionEntry.FindEntry(item);
                if (entry is not null)
                {
                    if (entry.State == EntityState.Detached)
                        entry.State = EntityState.Unchanged;

                    updater(entry);
                }
            }
    }
    public static void Upsert<TEntity, TItem>(this CollectionEntry<TEntity, TItem> collectionEntry, Func<TItem, bool> predicate, Func<TItem> itemFactory, Action<EntityEntry<TItem>> updateMatch, Action<EntityEntry<TItem>>? otherUpdater = null)
       where TEntity : class
       where TItem : class
    {
        if (collectionEntry.CurrentValue == null)
            collectionEntry.CurrentValue = [];

        bool found = false;
        foreach (TItem existing in collectionEntry.CurrentValue)
        {
            EntityEntry<TItem>? entry = collectionEntry.FindEntry(existing);
            if (entry is not null)
            {
                if (entry.State == EntityState.Detached)
                    entry.State = EntityState.Unchanged;

                if (predicate(existing))
                {
                    updateMatch(entry);
                    found = true;
                }
                otherUpdater?.Invoke(entry);
            }
        }
        if (!found)
        {
            collectionEntry.AddItem(itemFactory());
        }

    }

    public static void MergeBy<TEntity, TItem, TKey>(this CollectionEntry<TEntity, TItem> collectionEntry, Func<TItem, TKey> keySelector, ICollection<TItem> collection, Action<EntityEntry<TItem>, TItem> itemUpdate, IEqualityComparer<TKey>? comparer = null)
        where TEntity : class
        where TItem : class
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(keySelector);

        comparer ??= EqualityComparer<TKey>.Default;
        List<TItem> collectionEntries = [];
        if (collectionEntry.CurrentValue is null)
            collectionEntries.AddRange(collection);
        else
        {
            collectionEntries.AddRange(collectionEntry.CurrentValue);
            // restore remove add update using entry
            IEnumerable<TItem> toDelete = collectionEntries.ExceptBy(collection.Select(x => keySelector(x)), keySelector, comparer).ToList();
            foreach (TItem itemToDelete in toDelete)
            {
                collectionEntries.Remove(itemToDelete);
                EntityEntry<TItem>? entry = collectionEntry.FindEntry(itemToDelete);
                if (entry is not null)
                    entry.State = EntityState.Deleted;
            }

            foreach (TItem item in collection)
            {
                TItem? existing = collectionEntries.FirstOrDefault(x => comparer.Equals(keySelector(x), keySelector(item)));
                if (existing is not null)
                {
                    EntityEntry<TItem>? entry = collectionEntry.FindEntry(existing);
                    if (entry is not null)
                    {
                        if (entry.State == EntityState.Detached)
                            entry.State = EntityState.Unchanged;

                        itemUpdate(entry, item);
                    }
                }
                else
                {
                    collectionEntries.Add(item);
                }
            }
        }
        collectionEntry.CurrentValue = collectionEntries;
    }
    public static void AddItem<TEntity, TItem>(this CollectionEntry<TEntity, TItem> collectionEntry, TItem itemCollection)
        where TEntity : class
        where TItem : class
    {
        List<TItem> collectionEntries = [itemCollection];
        if (collectionEntry.CurrentValue is not null)
            collectionEntries.AddRange(collectionEntry.CurrentValue);
        collectionEntry.CurrentValue = collectionEntries;
    }

    public static void RemoveItem<TEntity, TItem>(this CollectionEntry<TEntity, TItem> collectionEntry, TItem itemCollection)
        where TEntity : class
        where TItem : class
    {
        if (collectionEntry.CurrentValue is not null)
        {
            List<TItem> collectionEntries = [.. collectionEntry.CurrentValue];
            collectionEntries.Remove(itemCollection);
            collectionEntry.CurrentValue = collectionEntries;
        }
    }


    public static void RemoveItemBy<TEntity, TItem, TKey>(this CollectionEntry<TEntity, TItem> collectionEntry, Func<TItem, TKey> keySelector, TKey key, IEqualityComparer<TKey>? comparer = null)
        where TEntity : class
        where TItem : class
        where TKey : notnull
    {
        comparer ??= EqualityComparer<TKey>.Default;
        if (collectionEntry.CurrentValue is not null)
        {
            List<TItem> collectionEntries = [.. collectionEntry.CurrentValue];
            var item = collectionEntries.FirstOrDefault(x => comparer.Equals(keySelector(x), key));
            if (item != null)
            {
                collectionEntries.Remove(item);
                collectionEntry.CurrentValue = collectionEntries;
            }
        }
    }

    public static void AddItems<TEntity, TItem>(this CollectionEntry<TEntity, TItem> collectionEntry, ICollection<TItem> itemsCollection, IEqualityComparer<TItem>? comparer = null)
        where TEntity : class
        where TItem : class
    {
        List<TItem> collectionEntries = [.. itemsCollection];
        if (collectionEntry.CurrentValue is not null)
            collectionEntries.AddRange(collectionEntry.CurrentValue);
        collectionEntry.CurrentValue = collectionEntries;
    }
    public static void UpdateSingleItem<TEntity, TItem>(
        this CollectionEntry<TEntity, TItem> collectionEntry,
        TItem updatedItem,
        Action<EntityEntry<TItem>, TItem> itemUpdate,
        IEqualityComparer<TItem>? comparer = null)
    where TEntity : class
    where TItem : class
    {
        // If the collection is null or empty, we can't update anything
        if (collectionEntry.CurrentValue == null || !collectionEntry.CurrentValue.Any())
        {
            throw new InvalidOperationException("Cannot update item in an empty or null collection.");
        }

        comparer ??= EqualityComparer<TItem>.Default;

        // Find the existing item in the collection
        TItem? existingItem = collectionEntry.CurrentValue.FirstOrDefault(x => comparer.Equals(x, updatedItem))
            ?? throw new InvalidOperationException("Item to update was not found in the collection.");

        // Create a new collection with just the updated item
        var singleItemCollection = new List<TItem> { updatedItem };

        // Use the existing Merge method to handle the update
        collectionEntry.Merge(
            singleItemCollection,
            itemUpdate,
            comparer
        );
    }

    public static void Merge<TEntity, TItem>(this CollectionEntry<TEntity, TItem> collectionEntry, ICollection<TItem> collection, Action<EntityEntry<TItem>, TItem> itemUpdate, IEqualityComparer<TItem>? comparer = null)
        where TEntity : class
        where TItem : class
    {
        comparer ??= EqualityComparer<TItem>.Default;
        List<TItem> collectionEntries = [];
        if (collectionEntry.CurrentValue is null)
            collectionEntries.AddRange(collection);
        else
        {
            collectionEntries.AddRange(collectionEntry.CurrentValue);
            // restore remove add update using entry
            IEnumerable<TItem> toDelete = collectionEntries.Except(collection, comparer).ToList();
            foreach (TItem itemToDelete in toDelete)
            {
                collectionEntries.Remove(itemToDelete);
                EntityEntry<TItem>? entry = collectionEntry.FindEntry(itemToDelete);
                if (entry is not null)
                    entry.State = EntityState.Deleted;
            }

            foreach (TItem item in collection)
            {
                TItem? existing = collectionEntries.FirstOrDefault(x => comparer.Equals(x, item));
                if (existing is not null)
                {
                    EntityEntry<TItem>? entry = collectionEntry.FindEntry(existing);
                    if (entry is not null)
                    {
                        itemUpdate(entry, item);
                        if (entry.State == EntityState.Detached)
                            entry.State = EntityState.Unchanged;
                    }
                }
                else
                {
                    collectionEntries.Add(item);
                }
            }
        }
        collectionEntry.CurrentValue = collectionEntries;
    }
}

//public interface IModelPersistenceManager<TContext>
//{
//    Task PersistModelAsync<TEntity>(
//        TEntity? model,
//        CancellationToken cancellationToken = default
//    );
//}

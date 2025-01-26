//using Toucan.Sdk.Core.Exceptions;

//namespace Toucan.Sdk.Core.Handlers.Abstractions;


//public abstract class BaseAggregateHandler<T, TId>(IAggregateRoot root)
//    where T : IIdentified<TId>, IDomainAggregate
//    where TId : struct
//{
//    protected async Task<T> CreateAsync(TId id, Func<T, Task> action, CancellationToken ct)
//    {
//        try
//        {
//            T aggregate = root.GetFactory<T, TId>().Invoke(id);
//            await Task.Run(async () =>
//            {
//                await action(aggregate);
//            }, ct);
//            await root.GetRepository<T, TId>().SaveAsync(aggregate, ct);
//            return aggregate;
//        }
//        catch (Exception ex)
//        {
//            throw new HandleException($"{nameof(CreateAsync)} fail", ex);
//        }
//    }
//    //protected async Task<T> CreateAsync(TId id, Func<T, ValueTask> action, CancellationToken ct)
//    //{
//    //    try
//    //    {
//    //        T aggregate = root.GetFactory<T, TId>().Invoke(id);
//    //        await Task.Run(async () =>
//    //        {
//    //            await action(aggregate);
//    //        }, ct);
//    //        await root.GetRepository<T, TId>().SaveAsync(aggregate, ct);
//    //        return aggregate;
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        throw new HandleException($"{nameof(CreateAsync)} fail", ex);
//    //    }
//    //}
//    protected async Task<T> ResolveAsync(TId id, CancellationToken ct)
//    {
//        try
//        {
//            return await root.GetRepository<T, TId>().GetAsync(id, ct);
//        }
//        catch (Exception ex)
//        {
//            throw new HandleException($"{nameof(ResolveAsync)} fail", ex);
//        }
//    }
//    protected async Task<T> UpsertAsync(TId id, Func<T, Task> action, CancellationToken ct)
//    {
//        try
//        {
//            T aggregate = await ResolveAsync(id, ct);
//            await Task.Run(async () =>
//            {
//                await action(aggregate);
//            }, ct);
//            await root.GetRepository<T, TId>().SaveAsync(aggregate, ct);
//            return aggregate;
//        }
//        catch (Exception ex)
//        {
//            throw new HandleException($"{nameof(UpsertAsync)} fail", ex);
//        }
//    }
//    protected async Task<T> UpsertFastAsync(TId id, Func<T, ValueTask> action, CancellationToken ct)
//    {
//        try
//        {
//            T aggregate = await ResolveAsync(id, ct);
//            await Task.Run(async () =>
//            {
//                await action(aggregate);
//            }, ct);
//            await root.GetRepository<T, TId>().SaveAsync(aggregate, ct);
//            return aggregate;
//        }
//        catch (Exception ex)
//        {
//            throw new HandleException($"{nameof(UpsertAsync)} fail", ex);
//        }
//    }
//    //protected async Task<T> GetOrCreateAsync(TId id, Func<T, ValueTask> action, CancellationToken ct)
//    //{
//    //    try
//    //    {
//    //        T aggregate = await ResolveAsync(id, ct);
//    //        aggregate ??= root.GetFactory<T, TId>().Invoke(id);
//    //        await Task.Run(async () =>
//    //        {
//    //            await action(aggregate);
//    //        }, ct);
//    //        await root.GetRepository<T, TId>().SaveAsync(aggregate, ct);
//    //        return aggregate;
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        throw new HandleException($"{nameof(GetOrCreateAsync)} fail", ex);
//    //    }
//    //}
//    protected async Task<T> GetOrCreateAsync(TId id, Func<T, Task> action, CancellationToken ct)
//    {
//        try
//        {
//            T aggregate = await ResolveAsync(id, ct);
//            aggregate ??= root.GetFactory<T, TId>().Invoke(id);
//            await Task.Run(async () =>
//            {
//                await action(aggregate);
//            }, ct);
//            await root.GetRepository<T, TId>().SaveAsync(aggregate, ct);
//            return aggregate;
//        }
//        catch (Exception ex)
//        {
//            throw new HandleException($"{nameof(GetOrCreateAsync)} fail", ex);
//        }
//    }

//}

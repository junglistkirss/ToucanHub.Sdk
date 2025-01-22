/*using Microsoft.Extensions.Logging;
using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface IStreamProjection<TId, TEvent, TProjection>
    where TId : struct
    where TProjection : notnull
    where TEvent : notnull
{
    Task HandleAsync(IEventStoreProjectionSession<TId, TProjection> session, EventSlice<TId, TEvent> eventSlice);
}

public interface IStreamRepository<TId, TEvent, TProjection>
    where TId : struct
    where TEvent : notnull
    where TProjection : notnull
{
    Task Persist<T>(TId id, Versioning version, TEvent[] events, CancellationToken token);
    Task Save(TId id, Versioning version, TProjection Projection, CancellationToken token);
    Task<TEvent[]> Rehydrate(TId id, CancellationToken token);
}



public sealed class StreamRepository<TId, TEventStrore, TEvent, TProjection>(
    TEventStrore eventStrore,
    ILogger<StreamRepository<TId, TEventStrore, TEvent, TProjection>> logger
    ) : IStreamRepository<TId, TEvent, TProjection>
    where TEventStrore : IEventStore<TId, TEvent, TProjection>
    where TId : struct
    where TProjection : notnull
    where TEvent : notnull
{
    public async Task<TEvent[]> Rehydrate(TId id, CancellationToken token)
    {
        logger.LogInformation("Begin refresh state");
        TEvent[] result = await eventStrore.ReadAsync(id, token);
        logger.LogInformation("Stream state loaded : {len} event(s)", result.Length);
        return result;
    }

    public async Task Persist<T>(TId id, Versioning version, TEvent[] events, CancellationToken token)
    {
        logger.LogError("Start persist stream {key}<{version}> {len} event(s)", id, version, events.Length);
        using IStorageTransaction transaction = await eventStrore.BeginTransactionAsync(token);
        try
        {
            StreamInfo<TId> info = await eventStrore.EnsureOpenAsync(id, typeof(T), token);
            Versioning last = await eventStrore.WriteAsync(id, version, events, token);
            Abstractions.EventStoreProjectionSession<TId, TProjection> session = new Abstractions.EventStoreProjectionSession<TId, TProjection>(eventStrore.ProjectAsync);
            EventSlice<TId, TEvent> eventSlice = new(info with { Version = last }, events);
            //foreach (IAggregateProjection<TAggregate, TId, TEvent, TProjection> inline in aggregateProjections)
            //    await inline.HandleAsync(session, eventSlice, concrete);
            await transaction.CommitAsync(token);
        }
        catch (EventStoreException ex)
        {
            await transaction.RollbackAsync(token);
            logger.LogError(ex, "Error persist stream {key}<{version}>", id, version);
            throw;
        }
        finally
        { }
    }

    public async Task Save(TId id, Versioning version, TProjection projection, CancellationToken token)
    {
        logger.LogError("Start project stream {key}<{version}>", id, version);
        using IStorageTransaction transaction = await eventStrore.BeginTransactionAsync(token);
        try
        {
            StreamInfo<TId> info = await eventStrore.EnsureOpenAsync(id, typeof(object), token);
            await eventStrore.ProjectAsync(id, version, projection, token);
            await transaction.CommitAsync(token);
        }
        catch (EventStoreException ex)
        {
            await transaction.RollbackAsync(token);
            logger.LogError(ex, "Error projecting stream {key}<{version}>", id, version);
            throw;
        }
        finally
        { }
    }
}*/
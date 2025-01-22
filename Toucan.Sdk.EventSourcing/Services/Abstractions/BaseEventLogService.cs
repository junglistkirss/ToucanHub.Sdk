using System.Linq.Expressions;
using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services.Abstractions;

public abstract class BaseEventLogService<TStreamKey, TEventData, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>
    : IEventLogService<TStreamKey, TEventData, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>
    where TStreamKey : struct
    where TEventData : notnull
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    private bool disposedValue;

    protected abstract Task<StreamInfo<TStreamKey>> CreateStreamIfNotExists(TStoredStream stream, CancellationToken cancellationToken = default);
    protected abstract Task RenameStreamAsync(TStreamKey streamId, string name, CancellationToken cancellationToken = default);
    protected abstract Task LockStreamAsync(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract Task<StreamInfo<TStreamKey>> ReadStreamInfo(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract Task<Versioning> ReadStreamVersion(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract Task<(ETag, Versioning)> WriteEventsAsync(TStreamKey streamId, IEnumerable<TStoredEvent> events, CancellationToken cancellationToken = default);
    protected abstract Task WriteProjectionAsync(TStreamKey streamId, TStoredProjection projection, CancellationToken cancellationToken = default);
    protected abstract Task<TStoredProjection?> ReadLastProjectionAsync(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract IAsyncEnumerable<TStoredEvent> ReadEventsAsync(TStreamKey streamId, Versioning fromEventVersion, Versioning toEventVersion, CancellationToken cancellationToken = default);
    protected abstract Task<StreamMetadata> ReadStreamMetadataAsync(TStreamKey streamId, CancellationToken cancellationToken);
    protected abstract Task<EventMetadata> ReadEventMetadataAsync(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken);
    protected abstract Task DeleteStreamAsync(TStreamKey streamId, CancellationToken cancellationToken);
    protected abstract Task DeleteEventAsync(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken);
    public virtual async Task<StreamMetadata> GetStreamMetadata(TStreamKey streamId, CancellationToken cancellationToken = default)
    {
        try
        {
            StreamMetadata metadata = await ReadStreamMetadataAsync(streamId, cancellationToken);
            return metadata;
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Retrieving stream version fails", ex);
        }
    }
    public virtual async Task<StreamInfo<TStreamKey>> CreateStream(TStoredStream stream, CancellationToken cancellationToken = default) => await CreateStreamIfNotExists(stream, cancellationToken);

    public virtual async Task<(ETag, Versioning)> AppendToStreamOptimistic(TStreamKey streamId, TStoredEvent[] events, CancellationToken cancellationToken = default)
    {
        try
        {
            return await WriteEventsAsync(streamId, events, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Appending events in stream fails", ex);
        }
    }
    public virtual async Task AppendProjectionOptimistic(TStreamKey streamId, TStoredProjection projection, CancellationToken cancellationToken = default)
    {
        try
        {
            await WriteProjectionAsync(streamId, projection, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Appending stream projection fails", ex);
        }
    }

    public virtual async Task DeleteEvent(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            await DeleteEventAsync(streamId, eventId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Deleting event fails", ex);
        }
    }
    public virtual async Task DeleteStream(TStreamKey streamId, CancellationToken cancellationToken = default)
    {
        try
        {
            await DeleteStreamAsync(streamId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Deleting stream fails", ex);
        }
    }

    public virtual IAsyncEnumerable<TStoredEvent> GetAllEvents(TStreamKey streamId)
    {
        try
        {
            return ReadEventsAsync(streamId, Versioning.Zero, Versioning.Max);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Retrieving all events from stream fails", ex);
        }
    }
    public virtual async Task<EventMetadata> GetEventMetadata(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            EventMetadata metadata = await ReadEventMetadataAsync(streamId, eventId, cancellationToken);

            if (metadata == EventMetadata.Empty)
                throw new EventStoreException("Missing metadata");

            return metadata;
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Retrieving stream version fails", ex);
        }
    }

    public virtual IAsyncEnumerable<TStoredEvent> GetEventsAfter(TStreamKey streamId, Versioning fromEventVersion)
    {
        try
        {
            return ReadEventsAsync(streamId, fromEventVersion, Versioning.Max);
        }
        catch (Exception ex)
        {
            throw new EventStoreException($"Retrieving events after {fromEventVersion} from stream fails", ex);
        }
    }
    public virtual IAsyncEnumerable<TStoredEvent> GetEventsBefore(TStreamKey streamId, Versioning beforeEventVersion)
    {
        try
        {
            return ReadEventsAsync(streamId, Versioning.Zero, beforeEventVersion);
        }
        catch (Exception ex)
        {
            throw new EventStoreException($"Retrieving events before {beforeEventVersion} from stream fails", ex);
        }
    }

    public virtual async Task<StreamInfo<TStreamKey>> GetStreamInfo(TStreamKey streamId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ReadStreamInfo(streamId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Retrieving stream info fails", ex);
        }
    }
    public virtual async Task<Versioning> GetStreamVersion(TStreamKey streamId, CancellationToken cancellationToken = default)
    {
        try
        {
            Versioning actualVersion = await ReadStreamVersion(streamId, cancellationToken);
            return actualVersion;
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Retrieving stream version fails", ex);
        }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: supprimer l'état managé (objets managés)
            }

            disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    public abstract IAsyncEnumerable<TStoredEvent> GetEvents(TStreamKey streamId, Expression<Func<TStoredEvent, bool>> predicate, int offset, int limit);
    public abstract IAsyncEnumerable<TStoredStream> GetKeys(Expression<Func<TStoredStream, bool>> predicate, int offset, int limit);
    public virtual async Task LockStream(TStreamKey streamId, CancellationToken ct)
    {
        try
        {
            await LockStreamAsync(streamId, ct);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Locking stream fails", ex);
        }
    }
    public virtual async Task RenameStream(TStreamKey streamId, Func<string> name, CancellationToken ct)
    {
        try
        {
            await RenameStreamAsync(streamId, name().Trim(), ct);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Renaming stream fails", ex);
        }
    }

    public abstract Task<IStorageTransaction> CreateLogStorageTransactionAsync(CancellationToken ct, Action? commitCallback = null);

    public async Task<TStoredProjection?> GetLastProjection(TStreamKey streamId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ReadLastProjectionAsync(streamId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Reading stream projection fails", ex);
        }
    }
}
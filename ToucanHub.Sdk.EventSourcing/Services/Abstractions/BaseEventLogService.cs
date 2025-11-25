using ToucanHub.Sdk.EventSourcing.Models;

namespace ToucanHub.Sdk.EventSourcing.Services.Abstractions;

public abstract class BaseEventLogService<TStreamKey, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>
    : IEventLogService<TStreamKey, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>
    where TStreamKey : struct
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    protected abstract Task<StreamInfo<TStreamKey>> CreateStreamIfNotExists(TStoredStream stream, CancellationToken cancellationToken = default);
    protected abstract Task LockStreamAsync(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract Task<StreamInfo<TStreamKey>> ReadStreamInfo(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract Task<Versioning> ReadStreamVersion(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract Task<StreamInfo<TStreamKey>> WriteEventsAsync(TStreamKey streamId, IEnumerable<TStoredEvent> events, CancellationToken cancellationToken = default);
    protected abstract Task WriteProjectionAsync(TStreamKey streamId, TStoredProjection projection, CancellationToken cancellationToken = default);
    protected abstract Task<TStoredProjection?> ReadLastProjectionAsync(TStreamKey streamId, CancellationToken cancellationToken = default);
    protected abstract IAsyncEnumerable<TStoredEvent> ReadEventsAsync(TStreamKey streamId, SearchEvents search, int offset, int limit, CancellationToken cancellationToken = default);
    protected abstract IAsyncEnumerable<TStoredProjection> ReadProjectionsAsync(SearchProjection search, int offset, int limit, CancellationToken cancellationToken = default);
    protected abstract IAsyncEnumerable<TStoredStream> ReadStreamsAsync(SearchStreams search, int offset, int limit, CancellationToken cancellationToken = default);
    protected abstract Task<StreamMetadata> ReadStreamMetadataAsync(TStreamKey streamId, CancellationToken cancellationToken);
    protected abstract Task<EventMetadata> ReadEventMetadataAsync(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken);
    protected abstract Task DeleteStreamAsync(TStreamKey streamId, CancellationToken cancellationToken);
    protected abstract Task DeleteEventAsync(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken);
    public async Task<StreamMetadata> GetStreamMetadata(TStreamKey streamId, CancellationToken cancellationToken = default)
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
    public async Task<StreamInfo<TStreamKey>> CreateStreamAsync(TStoredStream stream, CancellationToken cancellationToken = default) => await CreateStreamIfNotExists(stream, cancellationToken);

    public async Task<StreamInfo<TStreamKey>> AppendToStream(TStreamKey streamId, TStoredEvent[] events, CancellationToken cancellationToken = default)
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

    public async Task AppendProjection(TStreamKey streamId, TStoredProjection projection, CancellationToken cancellationToken = default)
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

    public async Task DeleteEvent(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default)
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

    public async Task DeleteStream(TStreamKey streamId, CancellationToken cancellationToken = default)
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

    public async Task<EventMetadata> GetEventMetadata(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default)
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

    public async Task<StreamInfo<TStreamKey>> GetStreamInfo(TStreamKey streamId, CancellationToken cancellationToken = default)
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

    public async Task<Versioning> GetStreamVersion(TStreamKey streamId, CancellationToken cancellationToken = default)
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

    public IAsyncEnumerable<TStoredEvent> GetEvents(TStreamKey streamId, SearchEvents predicate, int offset, int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            return ReadEventsAsync(streamId, predicate, offset, limit, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Reading events failed", ex);

        }
    }

    public IAsyncEnumerable<TStoredProjection> GetProjections(SearchProjection predicate, int offset, int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            return ReadProjectionsAsync(predicate, offset, limit, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Reading projections failed", ex);

        }
    }

    public IAsyncEnumerable<TStoredStream> GetStreams(SearchStreams predicate, int offset, int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            return ReadStreamsAsync(predicate, offset, limit, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new EventStoreException("Reading streams failed", ex);

        }
    }

    public async Task LockStream(TStreamKey streamId, CancellationToken ct)
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
using System.Data;
using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services.Abstractions;

public abstract class EventStore<TStreamKey, TEvent, TProjection, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>(
    IEventLogService<TStreamKey, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage> eventLogService,
    ISerializer<TStreamKey, TEvent, TProjection, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage> serializer
    ) :
    IEventStore<TStreamKey, TEvent>,
    ISnapshotStore<TStreamKey, TProjection>
    where TStreamKey : struct
    where TEvent : notnull
    where TProjection : notnull
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public virtual async Task<IStorageTransaction> BeginTransactionAsync(Action? commitCallback = null, CancellationToken ct = default)
    {
        try
        {
            IStorageTransaction tran = await eventLogService.CreateLogStorageTransactionAsync(ct, commitCallback);
            return tran;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public virtual async Task<StreamInfo<TStreamKey>> WriteAsync(TStreamKey key, Versioning expectedVersion, IReadOnlyCollection<TEvent> events, CancellationToken ct = default)
    {
        using (EventSourcingTelemetry.Start("write_events"))
            try
            {
                await semaphore.WaitAsync(ct);
                Versioning actualVersion = await eventLogService.GetStreamVersion(key, ct);
                if (actualVersion != expectedVersion)
                    throw new EventStoreException(@"Mismatch expected version expected:={@expectedVersion} actual:={@actualVersion}");

                Versioning next = actualVersion;
                next++;
                TStoredEvent[] inputEvents = events
                    .Select(x => serializer.Serialize(key, next++, x)).ToArray();

                return await eventLogService.AppendToStream(key, [.. inputEvents], ct);
            }
            finally
            {
                semaphore.Release();
            }
    }

    public virtual async Task DeleteAsync(TStreamKey key, Versioning expectedVersion, CancellationToken ct = default)
    {
        using (EventSourcingTelemetry.Start("delete_stream"))
            try
            {
                await semaphore.WaitAsync(ct);
                Versioning actualVersion = await eventLogService.GetStreamVersion(key, ct);
                if (actualVersion != expectedVersion)
                    throw new EventStoreException(@"Mismatch expected version expected:={@expectedVersion} actual:={@actualVersion}");

                await eventLogService.DeleteStream(key, ct);
            }
            finally
            {
                semaphore.Release();
            }
    }

    public virtual async Task<StreamInfo<TStreamKey>> EnsureOpenAsync(TStreamKey key, string? type, CancellationToken ct = default)
    {
        using (EventSourcingTelemetry.Start("open_stream"))
            try
            {
                await semaphore.WaitAsync(ct);
                StreamInfo<TStreamKey> info = await eventLogService.GetStreamInfo(key, ct);
                if (info.Version == Versioning.Any)
                {
                    TStoredStream stream = InitStream(key, type);
                    StreamInfo<TStreamKey> last = await eventLogService.CreateStreamAsync(stream, ct);
                    return last;
                }
                return info;
            }
            finally
            {
                semaphore.Release();
            }
    }

    protected abstract TStoredStream InitStream(TStreamKey key, string? type);

    public virtual async Task SaveSnapshotAsync(TStreamKey key, Versioning version, TProjection projection, CancellationToken ct = default)
    {
        using (EventSourcingTelemetry.Start("write_projection"))
            try
            {
                await semaphore.WaitAsync(ct);
                StreamInfo<TStreamKey> actual = await eventLogService.GetStreamInfo(key, ct);
                if (version > actual.Version)
                    throw new EventStoreException(@"Adding future projection is forbidden");

                TStoredProjection data = serializer.Serialize(key, version, projection);

                await eventLogService.AppendProjection(key, data, ct);
            }
            finally
            {
                semaphore.Release();
            }
    }

    public virtual async Task<EventSlice<TEvent>> ReadAsync(TStreamKey key, SearchEvents predicate, int offset, int limit, CancellationToken ct = default)
    {
        using (EventSourcingTelemetry.Start("read_events_offset"))
            try
            {
                await semaphore.WaitAsync(ct);
                HashSet<TEvent> output = [];
                Versioning version = Versioning.Zero;
                ETag tag = ETag.Empty;
                await foreach (TStoredEvent item in eventLogService.GetEvents(key, predicate, offset, limit, ct))
                {
                    if (!output.Add(serializer.Deserialize(item)))
                        throw new EventStoreException($"Duplicate event {item.Id}");

                    version = item.Position;
                    tag = item.Metadata.ETag;
                }
                return new EventSlice<TEvent>(version, tag, [.. output]);
            }
            finally
            {
                semaphore.Release();
            }
    }

    public virtual async Task<(Versioning Version, ETag ETag, TProjection? Data)> GetLastSnapshotAsync(TStreamKey key, CancellationToken ct = default)
    {
        using (EventSourcingTelemetry.Start("read_projection"))
            try
            {
                await semaphore.WaitAsync(ct);
                TStoredProjection? last = await eventLogService.GetLastProjection(key, ct);
                if (last is not null)
                    return (last.Offset, last.Metadata.ETag, serializer.Deserialize(last));
                return (Versioning.Zero, default, default);
            }
            finally
            {
                semaphore.Release();
            }
    }
}

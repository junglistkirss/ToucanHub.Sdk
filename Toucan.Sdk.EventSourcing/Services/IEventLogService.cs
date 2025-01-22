using System.Linq.Expressions;
using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface IEventLogService<TStreamKey, TEventData, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage> : IDisposable
    where TStreamKey : struct
    where TEventData : notnull
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    Task<IStorageTransaction> CreateLogStorageTransactionAsync(CancellationToken ct, Action? commitCallback = null);

    Task<StreamInfo<TStreamKey>> CreateStream(TStoredStream stream, CancellationToken ct);

    Task LockStream(TStreamKey streamId, CancellationToken ct);

    Task RenameStream(TStreamKey streamId, Func<string> name, CancellationToken ct);

    Task<(ETag, Versioning)> AppendToStreamOptimistic(TStreamKey streamId, TStoredEvent[] events, CancellationToken cancellationToken = default);

    Task DeleteStream(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task DeleteEvent(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default);

    Task<StreamMetadata> GetStreamMetadata(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task<StreamInfo<TStreamKey>> GetStreamInfo(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task<Versioning> GetStreamVersion(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task<EventMetadata> GetEventMetadata(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default);

    public Task<TStoredEvent[]> GetEventsAfter(TStreamKey streamId, Versioning fromEventVersion, CancellationToken cancellationToken = default) => Task.FromResult(GetEventsAfter(streamId, fromEventVersion).ToBlockingEnumerable(cancellationToken).ToArray());

    IAsyncEnumerable<TStoredEvent> GetEventsAfter(TStreamKey streamId, Versioning fromEventVersion);

    public Task<TStoredEvent[]> GetEventsBefore(TStreamKey streamId, Versioning beforeEventVersion, CancellationToken cancellationToken = default) => Task.FromResult(GetEventsBefore(streamId, beforeEventVersion).ToBlockingEnumerable(cancellationToken).ToArray());
    IAsyncEnumerable<TStoredEvent> GetEventsBefore(TStreamKey streamId, Versioning beforeEventVersion);

    public Task<TStoredEvent[]> GetEvents(TStreamKey streamId, Expression<Func<TStoredEvent, bool>> predicate, int offset, int limit, CancellationToken cancellationToken = default) => Task.FromResult(GetEvents(streamId, predicate, offset, limit).ToBlockingEnumerable(cancellationToken).ToArray());
    IAsyncEnumerable<TStoredEvent> GetEvents(TStreamKey streamId, Expression<Func<TStoredEvent, bool>> predicate, int offset, int limit);

    public Task<TStoredStream[]> GetKeys(Expression<Func<TStoredStream, bool>> predicate, int offset, int limit, CancellationToken cancellationToken = default) => Task.FromResult(GetKeys(predicate, offset, limit).ToBlockingEnumerable(cancellationToken).ToArray());
    IAsyncEnumerable<TStoredStream> GetKeys(Expression<Func<TStoredStream, bool>> predicate, int offset, int limit);

    public Task<TStoredEvent[]> GetAllEvents(TStreamKey streamId, CancellationToken cancellationToken = default) => Task.FromResult(GetAllEvents(streamId).ToBlockingEnumerable(cancellationToken).ToArray());
    IAsyncEnumerable<TStoredEvent> GetAllEvents(TStreamKey streamId);

    Task AppendProjectionOptimistic(TStreamKey streamId, TStoredProjection projection, CancellationToken cancellationToken = default);
    Task<TStoredProjection?> GetLastProjection(TStreamKey streamId, CancellationToken cancellationToken = default);
}

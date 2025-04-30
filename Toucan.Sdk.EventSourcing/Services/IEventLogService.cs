using System.Linq.Expressions;
using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface IEventLogService<TStreamKey, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>
    where TStreamKey : struct
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    Task<IStorageTransaction> CreateLogStorageTransactionAsync(CancellationToken ct, Action? commitCallback = null);

    Task<StreamInfo<TStreamKey>> CreateStreamAsync(TStoredStream stream, CancellationToken ct);

    Task LockStream(TStreamKey streamId, CancellationToken ct);

    Task<(ETag, Versioning)> AppendToStream(TStreamKey streamId, TStoredEvent[] events, CancellationToken cancellationToken = default);

    Task DeleteStream(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task DeleteEvent(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default);

    Task<StreamMetadata> GetStreamMetadata(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task<StreamInfo<TStreamKey>> GetStreamInfo(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task<Versioning> GetStreamVersion(TStreamKey streamId, CancellationToken cancellationToken = default);

    Task<EventMetadata> GetEventMetadata(TStreamKey streamId, Guid eventId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TStoredEvent> GetEvents(TStreamKey streamId, SearchEvents predicate, int offset, int limit, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TStoredProjection> GetProjections(SearchProjection predicate, int offset, int limit, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TStoredStream> GetStreams(SearchStreams predicate, int offset, int limit, CancellationToken cancellationToken = default);

    Task AppendProjection(TStreamKey streamId, TStoredProjection projection, CancellationToken cancellationToken = default);
    Task<TStoredProjection?> GetLastProjection(TStreamKey streamId, CancellationToken cancellationToken = default);
}

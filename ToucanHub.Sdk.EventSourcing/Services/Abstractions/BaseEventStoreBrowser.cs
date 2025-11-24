using ToucanHub.Sdk.EventSourcing.Models;

namespace ToucanHub.Sdk.EventSourcing.Services.Abstractions;

public abstract class BaseEventStoreBrowser<TStreamKey, TEvent, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>(
    IEventLogService<TStreamKey, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage> eventLogService) :
    IEventStoreBrowser<TStreamKey, TStoredStream, TStoredEvent, THeadersStorage, TEventDataStorage>,
    IEventStoreProjectionBrowser<TStreamKey, TStoredProjection, THeadersStorage, TProjectionDataStorage>
    where TStreamKey : struct
    where TEvent : notnull
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    public virtual IAsyncEnumerable<TStoredEvent> BrowseEventsAsync(TStreamKey streamId, SearchEvents search, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return eventLogService.GetEvents(streamId, search, offset, limit, cancellationToken);
    }

    public virtual IAsyncEnumerable<TStoredProjection> BrowseProjectionsAsync(SearchProjection search, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return eventLogService.GetProjections(search, offset, limit, cancellationToken);
    }

    public virtual IAsyncEnumerable<TStoredStream> BrowseStreamsAsync(SearchStreams search, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return eventLogService.GetStreams(search, offset, limit, cancellationToken);
    }
}
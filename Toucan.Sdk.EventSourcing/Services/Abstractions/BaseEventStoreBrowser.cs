using System.Linq.Expressions;
using System.Threading;
using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services.Abstractions;

public abstract class BaseEventStoreBrowser<TStreamKey, TEvent, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>(
    IEventLogService<TStreamKey,  TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage> eventLogService) :
    IEventStoreBrowser<TStreamKey, TStoredStream, TStoredEvent, THeadersStorage, TEventDataStorage>,
    IEventStoreProjectionBrowser<TStreamKey, TStoredProjection, THeadersStorage, TProjectionDataStorage>
    where TStreamKey : struct
    where TEvent : notnull
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    public IAsyncEnumerable<TStoredEvent> BrowseEventsAsync(TStreamKey streamId,  SearchEvents search, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return eventLogService.GetEvents(streamId, search, offset, limit, cancellationToken);
    }

    public IAsyncEnumerable<TStoredProjection> BrowseProjectionsAsync(SearchProjection search, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return eventLogService.GetProjections(search, offset, limit, cancellationToken);
    }

    public IAsyncEnumerable<TStoredStream> BrowseStreamsAsync(SearchStreams search, int offset, int limit,CancellationToken cancellationToken = default)
    {
        return eventLogService.GetKeys(search, offset, limit, cancellationToken);
    }
}
using System.Linq.Expressions;
using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services.Abstractions;

public abstract class BaseEventStoreBrowser<TStreamKey, TEvent, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage>(
    IEventLogService<TStreamKey, TEvent, TStoredStream, TStoredEvent, TStoredProjection, THeadersStorage, TEventDataStorage, TProjectionDataStorage> eventLogService) :
    IEventStoreBrowser<TStreamKey, TStoredStream, TStoredEvent, THeadersStorage, TEventDataStorage>
    where TStreamKey : struct
    where TEvent : notnull
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>
    where TStoredProjection : IStoredProjection<THeadersStorage, TProjectionDataStorage>
{
    public IAsyncEnumerable<TStoredEvent> BrowseEventsAsync(TStreamKey streamId, int offset, int limit, SearchEvents<TEventDataStorage>? search = null)
    {
        return eventLogService.GetEvents(streamId, GetExpression(search), offset, limit);
    }

    public IAsyncEnumerable<TStoredStream> BrowseStreamsAsync(int offset, int limit, SearchStreams? search = null)
    {
        return eventLogService.GetKeys(GetExpression(search), offset, limit);
    }

    protected abstract Expression<Func<TStoredEvent, bool>> GetExpression(SearchEvents<TEventDataStorage>? search);
    protected abstract Expression<Func<TStoredStream, bool>> GetExpression(SearchStreams? search);
}
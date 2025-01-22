using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface IEventStoreProjectionBrowser<TStreamKey, TStoredProjection, THeaders, TStorageData>
    where TStreamKey : struct
    where TStoredProjection : IStoredProjection<THeaders, TStorageData>

{
    IAsyncEnumerable<TStoredProjection> BrowseProjectionsAsync(int offset, int limit);
    IAsyncEnumerable<TStoredProjection> BrowseProjectionsAsync(TStreamKey streamId, ProjectionsFilter<TStorageData> search, int offset, int limit);
}

public sealed record class ProjectionsFilter<TStorageData>
{

}

public interface IEventStoreBrowser<TStreamKey, TStoredStream, TStoredEvent, THeadersStorage, TEventDataStorage>
    where TStreamKey : struct
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>

{
    IAsyncEnumerable<TStoredStream> BrowseStreamsAsync(int offset, int limit, SearchStreams? search = null);
    IAsyncEnumerable<TStoredEvent> BrowseEventsAsync(TStreamKey streamId, int offset, int limit, SearchEvents<TEventDataStorage>? search = null);
}

public sealed record class SearchEvents<TStorageData> { }
public sealed record class SearchStreams { }


using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface IEventStoreBrowser<TStreamKey, TStoredStream, TStoredEvent, THeadersStorage, TEventDataStorage>
    where TStreamKey : struct
    where TStoredStream : IStoredStream<TStreamKey>
    where TStoredEvent : IStoredEvent<THeadersStorage, TEventDataStorage>

{
    IAsyncEnumerable<TStoredEvent> BrowseEventsAsync(TStreamKey streamId, SearchEvents search, int offset, int limit, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TStoredStream> BrowseStreamsAsync(SearchStreams search, int offset, int limit, CancellationToken cancellationToken = default);
}


using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface IEventStore<TStreamKey, TEvent> : IEventStoreReader<TStreamKey, TEvent>, IEventStoreWriter<TStreamKey, TEvent>
    where TStreamKey : struct
    where TEvent : notnull
{
    Task<IStorageTransaction> BeginTransactionAsync(Action? commitCallback = null, CancellationToken ct = default);
    Task<StreamInfo<TStreamKey>> EnsureOpenAsync(TStreamKey key, string typeName, CancellationToken ct = default);
}
public interface IEventStoreReader<TStreamKey, TEvent>
    where TStreamKey : struct
    where TEvent : notnull
{
    Task<EventSlice<TEvent>> ReadAsync(TStreamKey key, CancellationToken ct = default);
    Task<EventSlice<TEvent>> ReadAsync(TStreamKey key, Versioning offset, CancellationToken ct = default);
}

public interface IEventStoreWriter<TStreamKey, TEvent>
   where TStreamKey : struct
   where TEvent : notnull
{
    Task<(ETag, Versioning)> WriteAsync(TStreamKey key, Versioning expectedVersion, IReadOnlyCollection<TEvent> events, CancellationToken ct = default);
    Task DeleteAsync(TStreamKey key, Versioning expectedVersion, CancellationToken ct = default);
}

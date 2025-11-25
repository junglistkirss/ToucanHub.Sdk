using ToucanHub.Sdk.EventSourcing.Models;

namespace ToucanHub.Sdk.EventSourcing.Services;

public interface IEventStore<TStreamKey, TEvent> : IEventStoreReader<TStreamKey, TEvent>, IEventStoreWriter<TStreamKey, TEvent>
    where TStreamKey : struct
    where TEvent : notnull
{
    Task<IStorageTransaction> BeginTransactionAsync(Action? commitCallback = null, CancellationToken ct = default);
    Task<StreamInfo<TStreamKey>> EnsureOpenAsync(TStreamKey key, string? type, CancellationToken ct = default);
}
public interface IEventStoreReader<TStreamKey, TEvent>
    where TStreamKey : struct
    where TEvent : notnull
{
    Task<EventSlice<TEvent>> ReadAsync(TStreamKey key, SearchEvents predicate, int offset, int limit, CancellationToken ct = default);
}

public record class SearchEvents
{
    public readonly static SearchEvents Empty = new();

    public Versioning? Before { get; init; }
    public Versioning? After { get; init; }
}

public interface IEventStoreWriter<TStreamKey, TEvent>
   where TStreamKey : struct
   where TEvent : notnull
{
    Task<StreamInfo<TStreamKey>> WriteAsync(TStreamKey key, Versioning expectedVersion, IReadOnlyCollection<TEvent> events, CancellationToken ct = default);
    Task DeleteAsync(TStreamKey key, Versioning expectedVersion, CancellationToken ct = default);
}

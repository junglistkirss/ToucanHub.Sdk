using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface ISnapshotStore<TStreamKey, TSnapshot> : ISnapshotStoreReader<TStreamKey, TSnapshot>, ISnapshotStoreWriter<TStreamKey, TSnapshot>
    where TSnapshot : notnull
    where TStreamKey : struct
{ }
public interface ISnapshotStoreWriter<TStreamKey, TSnapshot>
   where TSnapshot : notnull
   where TStreamKey : struct
{
    Task SaveSnapshotAsync(TStreamKey key, Versioning version, TSnapshot projection, CancellationToken ct = default);
}

public interface ISnapshotStoreReader<TStreamKey, TSnapshot>
   where TSnapshot : notnull
   where TStreamKey : struct
{
    Task<(Versioning Version, ETag ETag, TSnapshot? Data)> GetLastSnapshotAsync(TStreamKey key, CancellationToken ct = default);
}

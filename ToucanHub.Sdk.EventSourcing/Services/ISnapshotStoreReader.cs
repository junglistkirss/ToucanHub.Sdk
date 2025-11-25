using ToucanHub.Sdk.EventSourcing.Models;

namespace ToucanHub.Sdk.EventSourcing.Services;

public interface ISnapshotStoreReader<TStreamKey, TSnapshot>
   where TSnapshot : notnull
   where TStreamKey : struct
{
    Task<(Versioning Version, ETag ETag, TSnapshot? Data)> GetLastSnapshotAsync(TStreamKey key, CancellationToken ct = default);
}

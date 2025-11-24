using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;

public interface ISnapshotStoreReader<TStreamKey, TSnapshot>
   where TSnapshot : notnull
   where TStreamKey : struct
{
    Task<(Versioning Version, ETag ETag, TSnapshot? Data)> GetLastSnapshotAsync(TStreamKey key, CancellationToken ct = default);
}

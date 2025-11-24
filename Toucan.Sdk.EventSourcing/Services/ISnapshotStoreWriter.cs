using Toucan.Sdk.EventSourcing.Models;

namespace Toucan.Sdk.EventSourcing.Services;
public interface ISnapshotStoreWriter<TStreamKey, TSnapshot>
   where TSnapshot : notnull
   where TStreamKey : struct
{
    Task SaveSnapshotAsync(TStreamKey key, Versioning version, TSnapshot projection, CancellationToken ct = default);
}

using ToucanHub.Sdk.EventSourcing.Models;

namespace ToucanHub.Sdk.EventSourcing.Services;
public interface ISnapshotStoreWriter<TStreamKey, TSnapshot>
   where TSnapshot : notnull
   where TStreamKey : struct
{
    Task SaveSnapshotAsync(TStreamKey key, Versioning version, TSnapshot projection, CancellationToken ct = default);
}

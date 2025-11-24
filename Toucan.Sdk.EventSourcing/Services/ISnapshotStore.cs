namespace Toucan.Sdk.EventSourcing.Services;

public interface ISnapshotStore<TStreamKey, TSnapshot> : ISnapshotStoreReader<TStreamKey, TSnapshot>, ISnapshotStoreWriter<TStreamKey, TSnapshot>
    where TSnapshot : notnull
    where TStreamKey : struct
{ }

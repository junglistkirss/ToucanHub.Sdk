namespace Toucan.Sdk.Core.Aggregates;

public interface ISnapshotable<TSnapshot, TState>
    where TSnapshot : notnull
    where TState : class
{
    TSnapshot GetSnapshot();
    TState FromSnapshot(TSnapshot snapshot);
}

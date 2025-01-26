namespace Toucan.Sdk.Core.Aggregates;

public interface IPersistableAggregate
{
    long CurrentVersion { get; }

    bool IsNew { get; }
    bool IsInitialized { get; }
    bool IsDeleted { get; }
}

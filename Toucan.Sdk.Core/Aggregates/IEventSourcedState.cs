namespace Toucan.Sdk.Core.Aggregates;

public interface IEventSourcedState
{
    long CurrentVersion { get; }

}
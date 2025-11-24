namespace ToucanHub.Sdk.EventSourcing.Models;
public interface IStoredEvent<THeaders, TData>
{
    Guid Id { get; }
    TData EventData { get; }
    THeaders EventHeaders { get; }
    EventMetadata Metadata { get; }
    Versioning Position { get; }
}

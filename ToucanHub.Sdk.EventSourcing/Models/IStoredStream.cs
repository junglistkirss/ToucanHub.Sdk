namespace ToucanHub.Sdk.EventSourcing.Models;

public interface IStoredStream<TStreamKey>
    where TStreamKey : struct
{
    TStreamKey Id { get; }
    Versioning Version { get; }
    StreamMetadata Metadata { get; }
}

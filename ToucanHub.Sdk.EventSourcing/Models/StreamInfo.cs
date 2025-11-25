namespace ToucanHub.Sdk.EventSourcing.Models;

public readonly record struct StreamInfo<TStreamKey>(TStreamKey Key, StreamMetadata Metadata, Versioning Version)
    where TStreamKey : struct
{

}

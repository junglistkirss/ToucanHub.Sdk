namespace Toucan.Sdk.EventSourcing.Models;

public readonly record struct StreamMetadata
{
    public static readonly StreamMetadata Empty = new()
    {
        Locked = false,
        ETag = [],
        Created = DateTimeOffset.MinValue,
    };

    public DateTimeOffset Created { get; init; }
    public ETag ETag { get; init; }
    public bool Locked { get; init; }
    public bool Deleted { get; init; }
}

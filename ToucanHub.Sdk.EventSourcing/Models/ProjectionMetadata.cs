namespace ToucanHub.Sdk.EventSourcing.Models;

public readonly record struct ProjectionMetadata
{
    public static readonly ProjectionMetadata Empty = new()
    {
        ETag = [],
        TypeName = null,
        Timestamp = DateTimeOffset.MinValue,
    };

    public required ETag ETag { get; init; }
    public string? TypeName { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}

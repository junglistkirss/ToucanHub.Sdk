namespace ToucanHub.Sdk.EventSourcing.Models;

public readonly record struct EventMetadata
{
    public static readonly EventMetadata Empty = new()
    {
        ETag = [],
        Timestamp = DateTimeOffset.MinValue,
        Origin = null,
        Sender = null,
    };

    public ETag ETag { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public string? Origin { get; init; }
    public string? Sender { get; init; }
}

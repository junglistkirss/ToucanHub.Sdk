namespace Toucan.Sdk.Shared.Models;

public sealed record class StreamEnriched(DomainId StreamId, string StreamName, long PreviousVersion, long CurrerntVersion, string Etag, Commit[] Events);

public record class EventInfo
{
    public Guid EventId { get; init; } = default!;
    public long EventPosition { get; init; } = default!;
    public Commit EventCommit { get; init; } = default!;

}

public readonly record struct Commit
{
    public required MessageHeaders Headers { get; init; }
    public required EventMessage[] Messages { get; init; }
}

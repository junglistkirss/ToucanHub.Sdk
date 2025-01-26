
namespace Toucan.Sdk.Shared.Models;

public record class Versioned<T>
    where T : class
{
    public required T Model { get; init; } = default!;
    public required long Version { get; init; }
}

public record class SubscriptionPolicy
{
    public bool IsManualEnabled { get; init; }
    public TimeSpan[] Retry { get; init; } = [];

}

public record class SubscriptionTrigger
{
    public TimeSpan? Delay { get; init; }
    public DateTimeOffset? ScheduledStart { get; init; }
    public DateTimeOffset? ScheduledEnd { get; init; }
    public int RepeatCount { get; init; }
    public TimeSpan? Interval { get; init; }
}

public record class Subscription
{
    public DomainId Key { get; init; }
    public Slug Name { get; init; }
    public string? RawOptions { get; init; }
    public SubscriptionSharedOption[] SharedRawOptions { get; init; } = [];
    public SubscriptionTrigger Trigger { get; init; } = default!;
    public SubscriptionPolicy Policy { get; init; } = default!;
    public string? JobName { get; init; }
}

public record class SubscriptionSharedOption
{
    public Slug Name { get; init; }
    public string RawOptions { get; init; } = default!;
}
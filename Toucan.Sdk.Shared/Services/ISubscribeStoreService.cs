using Toucan.Sdk.Shared.Models;

namespace Toucan.Sdk.Shared.Services;


public interface ISubscriptionService
{
    IAsyncEnumerable<Subscription> GetSubscriptions(DomainId aggregateId, params string[] trigger);
}
public interface ISubscribeStoreService
{
    ValueTask SetEnabledStateSubscribe(DomainId subscribeId, bool isEnabled, CancellationToken cancellationToken);
    ValueTask ConfigureSubscribe(DomainId subscribeId, string actionJobName, string? options, CancellationToken cancellationToken);
    ValueTask CreateSubscribe(DomainId subscribeId, Slug name, CancellationToken cancellationToken);
    ValueTask DeleteAsync(DomainId subscribeId, CancellationToken cancellationToken);
    ValueTask SetPredicateAsync(DomainId subscribeId, DomainId[] aggregateIds, string[] eventTypeNames, CancellationToken cancellationToken);
    ValueTask SetSubscribeTrigger(DomainId subscribeId, int repeatCount, TimeSpan? interval, DateTimeOffset? scheduledStart, DateTimeOffset? scheduledEnd, TimeSpan? delay, CancellationToken args);
    ValueTask SetSubscribePolicy(DomainId subscribeId, TimeSpan[] retry, bool manualTriggerEnabled, CancellationToken args);
}

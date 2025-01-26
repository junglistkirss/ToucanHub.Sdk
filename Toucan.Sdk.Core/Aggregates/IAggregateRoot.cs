using Toucan.Sdk.Shared.Models;

namespace Toucan.Sdk.Core.Aggregates;

public interface IAggregateRoot
{
    ValueTask<T> GetOrCreate<T, TId>(TId id, CancellationToken token = default)
        where T : IIdentified<TId>, IAggregate
        where TId : struct;
    ValueTask<T?> Resolve<T, TId>(TId id, CancellationToken token = default)
        where T : IIdentified<TId>, IAggregate
        where TId : struct;
    ValueTask Save<T, TId>(T aggregate, CancellationToken token = default)
        where T : IIdentified<TId>, IAggregate
        where TId : struct;
}


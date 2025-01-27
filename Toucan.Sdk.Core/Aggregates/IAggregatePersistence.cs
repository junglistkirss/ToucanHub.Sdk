using Toucan.Sdk.Shared.Models;

namespace Toucan.Sdk.Core.Aggregates;

public interface IAggregatePersistence<TAggregate, TId>
    where TAggregate : IIdentified<TId>, IAggregate
    where TId : struct
{
    ValueTask<TAggregate> GetAsync(TId id, CancellationToken token);
    ValueTask SaveAsync(TAggregate aggregate, CancellationToken token);

    ValueTask DeleteAsync(TAggregate aggregate, CancellationToken token);
}


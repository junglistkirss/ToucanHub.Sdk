using Toucan.Sdk.Contracts.Wrapper;

namespace Toucan.Sdk.Contracts.Messages;

public abstract record class GetBy<TId, TOut> : QueryMessage<TOut>
    where TId : struct
{
    public TId Key { get; init; }
}
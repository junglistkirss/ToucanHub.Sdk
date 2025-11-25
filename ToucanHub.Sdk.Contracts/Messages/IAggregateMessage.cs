namespace ToucanHub.Sdk.Contracts.Messages;

public interface IAggregateMessage<TId>
    where TId : struct
{
    TId AggregateId { get; }
}
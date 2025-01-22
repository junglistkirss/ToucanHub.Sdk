namespace Toucan.Sdk.Contracts.Messages;

public abstract record class EventMessage : IMessage { }

//public record class EventBatch : EventMessage
//{
//    public EventMessage[] Events { get; init; } = [];
//}

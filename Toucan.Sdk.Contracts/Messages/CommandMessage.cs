namespace Toucan.Sdk.Contracts.Messages;

public abstract record class CommandMessage : IMessage { }

public record class CommandBatch : CommandMessage
{
    public CommandMessage[] Events { get; init; } = [];
}

namespace Toucan.Sdk.Contracts.Messages;

public abstract record class QueryMessage : IMessage { }
public abstract record class QueryMessage<TResponse> : QueryMessage { }

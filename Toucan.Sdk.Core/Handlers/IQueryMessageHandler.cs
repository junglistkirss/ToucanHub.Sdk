using Toucan.Sdk.Contracts.Wrapper;
using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Shared.Messages;

namespace Toucan.Sdk.Core.Handlers;

public interface IQueryMessageHandler<TMessage, TResponse> : IQueryHandler<QueryEnvelope<TMessage>, TResponse>
    where TMessage : QueryMessage<TResponse>
    where TResponse : Result
{ }
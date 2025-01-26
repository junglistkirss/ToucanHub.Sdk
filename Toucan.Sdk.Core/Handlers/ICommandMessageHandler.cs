using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Shared.Messages;

namespace Toucan.Sdk.Core.Handlers;

public interface ICommandMessageHandler<TMessage> : ICommandHandler<CommandEnvelope<TMessage>>
    where TMessage : CommandMessage
{

}

public interface ICommandResponseMessageHandler<TMessage, TResponse> : ICommandHandler<CommandEnvelope<TMessage>, TResponse>
    where TMessage : CommandMessage
    where TResponse : class
{

}

//public interface ICommandMessageHandler<TMessage, TResponse> : ICommandHandler<CommandEnvelope<TMessage>, Feedback<TResponse>>, ICommandMessageHandler<TMessage>
//    where TMessage : ICommandMessage
//    where TResponse : IResult
//{
//    /// <summary>
//    /// Discard handler response
//    /// </summary>
//    /// <param name="command"></param>
//    /// <param name="ct"></param>
//    /// <returns></returns>
//    async Task ICommandHandler<CommandEnvelope<TMessage>>.HandleAsync(CommandEnvelope<TMessage> command, CancellationToken ct) =>
//        _ = await ((ICommandHandler<CommandEnvelope<TMessage>, Feedback<TResponse>>)this).HandleAsync(command, ct);
//}


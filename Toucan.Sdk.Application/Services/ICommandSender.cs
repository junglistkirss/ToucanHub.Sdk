using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Contracts.Wrapper;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Services;
public interface ICommandSender
{
    public ValueTask SendCommand<T>(T message)
        where T : class, ICommand;

    public ValueTask<TResponse> SendCommandWait<T, TResponse>(T message)
        where T : class, ICommand
        where TResponse : Result;

    public ValueTask SendCommandMessage<T>(T message)
        where T : CommandMessage;

    public ValueTask<TResponse> SendCommandMessageWait<T, TResponse>(T message)
        where T : CommandMessage
        where TResponse : Result;
}


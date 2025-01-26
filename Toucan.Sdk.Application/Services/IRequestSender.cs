using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Contracts.Wrapper;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Services;
public interface IRequestSender
{
    public ValueTask<TResponse> SendQueryMessage<T, TResponse>(T message)
        where T : QueryMessage<TResponse>
        where TResponse : Result;

    public ValueTask<TResponse> SendQuery<T, TResponse>(T message)
        where T : class, IQuery
        where TResponse : Result;


}


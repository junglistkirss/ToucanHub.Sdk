namespace Toucan.Sdk.Application.Broker;

public interface ITransportBus : ITransportQueryBus, ITransportCommandBus, ITransportEventBus
{
    ValueTask PostAsync<T>(T e, CancellationToken cancellationToken = default) where T : class;
}

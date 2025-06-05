namespace Toucan.Sdk.Reactive;

public interface IReactiveManagedDispatcher<TServiceId>
    where TServiceId : IEquatable<TServiceId>
{
    void Publish<T>(TServiceId serviceId, T value);
    void Complete(TServiceId serviceId);
}

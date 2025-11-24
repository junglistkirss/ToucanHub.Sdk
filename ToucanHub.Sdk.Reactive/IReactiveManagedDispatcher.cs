namespace ToucanHub.Sdk.Reactive;

public interface IReactiveManagedDispatcher<TServiceId>
    where TServiceId : struct
{
    void Publish<T>(TServiceId serviceId, T value);
    void Throw(TServiceId serviceId, Exception exception);
    void Complete(TServiceId serviceId);
}

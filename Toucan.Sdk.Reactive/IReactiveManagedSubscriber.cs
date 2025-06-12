namespace Toucan.Sdk.Reactive;

public interface IReactiveManagedSubscriber<TServiceId> 
    where TServiceId : struct
{
    IObservable<T> Observe<T>(TServiceId serviceId);

    IDisposable Register(TServiceId serviceId, IDisposable disposable);
    IDisposable Register<T>(TServiceId serviceId, Func<IObservable<T>, IDisposable> configure);
}

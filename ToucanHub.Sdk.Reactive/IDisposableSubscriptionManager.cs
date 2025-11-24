namespace ToucanHub.Sdk.Reactive;

public interface IDisposableSubscriptionManager
{
    IDisposable Register(IDisposable disposable);
    IDisposable Register<T>(Func<IObservable<T>, IDisposable> configure);
}


namespace ToucanHub.Sdk.Reactive;
public interface ISubscriptionManager : IDisposableSubscriptionManager
{
    Task<bool> WaitForStart(CancellationToken cancellationToken = default);
    IObservable<T> Observe<T>();
}


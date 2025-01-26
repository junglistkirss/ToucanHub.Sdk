namespace Toucan.Sdk.Reactive;


public interface ISubscriptionManager
{
    Task<bool> WaitForStart(CancellationToken cancellationToken = default);

    IDisposable Subscribe<T>(Action<T> handler);
    IDisposable Subscribe<T>(Func<T, ValueTask> handler);

    IObservable<T> Observe<T>();
    IDisposable Subscribe<T, TTransform>(Func<IObservable<T>, IObservable<TTransform>> transform);
}


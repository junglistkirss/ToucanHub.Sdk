namespace Toucan.Sdk.Reactive;


public interface ISubscriptionManager
{
    Task<bool> WaitForStart(CancellationToken cancellationToken = default);

    IDisposable Subscribe<T>(Action<T> handler, Action<Exception>? error = null, Action? complete = null);
    IDisposable Subscribe<T>(Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null);
    IDisposable Subscribe<T>(Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null);

    IObservable<T> Observe<T>();
    IDisposable Subscribe<T, TTransform>(Func<IObservable<T>, IObservable<TTransform>> transform);
}


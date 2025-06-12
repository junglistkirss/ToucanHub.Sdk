namespace Toucan.Sdk.Reactive;

public interface IReactiveManagedSubscriber<TServiceId>
    where TServiceId : struct
{
    IDisposable Subscribe<T>(TServiceId serviceId, Action<T> handler, Action<Exception>? error = null, Action? complete = null);
    IDisposable Subscribe<T>(TServiceId serviceId, Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null);
    IDisposable Subscribe<T>(TServiceId serviceId, Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null);
}

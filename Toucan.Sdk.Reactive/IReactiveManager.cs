namespace Toucan.Sdk.Reactive;

internal interface IReactiveManager
{
    IDisposable Subscribe<T>(Action<T> handler, Action<Exception>? error = null, Action? complete = null);
    IDisposable Subscribe<T>(Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null);
    IDisposable Subscribe<T>(Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null);
}

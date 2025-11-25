using System.Runtime.CompilerServices;

namespace ToucanHub.Sdk.Async;

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory) :
        base(() => Task.Factory.StartNew(valueFactory))
    { }
    public AsyncLazy(Func<ValueTask<T>> taskFactory) :
       base(() => Task.Factory.StartNew(() => taskFactory().AsTask()).Unwrap())
    { }
    public AsyncLazy(Func<Task<T>> taskFactory) :
        base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
    { }

    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();

}

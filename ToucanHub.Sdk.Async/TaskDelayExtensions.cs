namespace ToucanHub.Sdk.Async;

public static class TaskDelayExtensions
{
    public static void WithShortDelay(this Task originalTask, int millisecondsDelay = 100)
    {
        if (originalTask.IsCompleted)
        {
            return;
        }

        using CancellationTokenSource cts = new();
        Task delayTask = Task.Delay(millisecondsDelay, cts.Token);

        Task.WhenAny(originalTask, delayTask).Wait();
        if (originalTask.IsCompleted)
        {
            cts.Cancel();
        }
        else
        {
            _ = originalTask.ConfigureAwait(false);
        }
    }

    public static void WithShortDelay(this ValueTask originalTask, int millisecondsDelay = 100)
    {
        if (originalTask.IsCompleted)
        {
            return;
        }

        using CancellationTokenSource cts = new();
        Task delayTask = Task.Delay(millisecondsDelay, cts.Token);

        Task.WhenAny(originalTask.AsTask(), delayTask).Wait();
        if (originalTask.IsCompleted)
        {
            cts.Cancel();
        }
        else
        {
            _ = originalTask.ConfigureAwait(false);
        }
    }

    public static void WithShortDelay<T>(this Task<T> originalTask, int millisecondsDelay = 100)
    {
        if (originalTask.IsCompleted)
        {
            return;
        }

        using CancellationTokenSource cts = new();
        Task delayTask = Task.Delay(millisecondsDelay, cts.Token);

        Task.WhenAny(originalTask, delayTask).Wait();
        if (originalTask.IsCompleted)
        {
            cts.Cancel();
        }
        else
        {
            _ = originalTask.ConfigureAwait(false);
        }
    }

    public static void WithShortDelay<T>(this ValueTask<T> originalTask, int millisecondsDelay = 100)
    {
        if (originalTask.IsCompleted)
        {
            return;
        }

        using CancellationTokenSource cts = new();
        Task delayTask = Task.Delay(millisecondsDelay, cts.Token);

        Task.WhenAny(originalTask.AsTask(), delayTask).Wait();
        if (originalTask.IsCompleted)
        {
            cts.Cancel();
        }
        else
        {
            _ = originalTask.ConfigureAwait(false);
        }
    }

}
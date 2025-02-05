using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;

namespace Toucan.Sdk.Reactive;

public interface ISubscrptionsSchedulerProvider
{
    IScheduler Scheduler { get; }
}

public sealed class ReactiveSubscriptionException : Exception
{
    public ReactiveSubscriptionException()
    {
    }

    public ReactiveSubscriptionException(string? message) : base(message)
    {
    }

    public ReactiveSubscriptionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

internal class ReactiveSubscriptions(ILogger<ReactiveSubscriptions> logger, ISubscrptionsSchedulerProvider? subscrptionsSchedulerProvider = null) : IHostedService, ISubscriptionDispatcher, ISubscriptionManager, IDisposable
{
    private readonly Subject<object> subject = new();
    private readonly IScheduler scheduler = subscrptionsSchedulerProvider?.Scheduler ?? TaskPoolScheduler.Default;
    private readonly CompositeDisposable subscriptions = new();
    private readonly object _lock = new();
    private TaskCompletionSource<bool> isStarted = new();
    private int isStarting;
    public Task<bool> WaitForStart(CancellationToken cancellationToken = default)
    {
        return isStarted.Task.WaitAsync(cancellationToken);
    }

    public void Publish<T>(T @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        if (isStarted.Task.IsCompletedSuccessfully)
        {
            subject.OnNext(@event);
        }
        else
        {
            logger.LogWarning("Attempted to publish event when service is not started");
        }
    }

    public void Complete()
    {
        if (isStarted.Task.IsCompletedSuccessfully)
        {
            subject.OnCompleted();
        }
        else
        {
            logger.LogWarning("Attempted to complete when service is not started");
        }
    }

    public IDisposable Subscribe<T>(Action<T> handler, Action<Exception>? error = null, Action? complete = null)
    {
        ArgumentNullException.ThrowIfNull(handler);

        IDisposable subscription = subject
            .OfType<T>()
            .ObserveOn(scheduler)
            .SubscribeOn(scheduler)
            .Subscribe(
                @event =>
                {
                    try
                    {
                        handler(@event);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error handling event of type {EventType}", typeof(T));
                        throw new ReactiveSubscriptionException("Error handling event", ex);
                    }
                },
                ex =>
                {
                    if (error is not null)
                        error(ex);
                    logger.LogError(ex, "Error in event stream for {EventType}", typeof(T));
                },
                () =>
                {
                    if (complete is not null)
                        complete();
                    logger.LogInformation("Completed in event stream for {EventType}", typeof(T));
                }
            );

        // Wrap subscription to ensure proper cleanup
        IDisposable managedSubscription = CreateManagedSubscription(subscription);
        return managedSubscription;
    }

    public IDisposable Subscribe<T>(Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null)
    {
        ArgumentNullException.ThrowIfNull(handler);

        IDisposable subscription = subject
            .OfType<T>()
            .ObserveOn(scheduler)
            .SubscribeOn(scheduler)
            .Subscribe(
                async @event =>
                {
                    try
                    {
                        await handler(@event);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error handling event of type {EventType}", typeof(T));
                        throw new ReactiveSubscriptionException("Error handling event", ex);
                    }
                },
                async ex =>
                {
                    if (error is not null)
                        await error(ex);
                    logger.LogError(ex, "Error in event stream for {EventType}", typeof(T));
                },
                async () =>
                {
                    if (complete is not null)
                        await complete();
                    logger.LogInformation("Completed in event stream for {EventType}", typeof(T));
                }
            );
        // Wrap subscription to ensure proper cleanup
        IDisposable managedSubscription = CreateManagedSubscription(subscription);
        return managedSubscription;
    }


    public IDisposable Subscribe<T>(Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null)
    {
        ArgumentNullException.ThrowIfNull(handler);

        IDisposable subscription = subject
            .OfType<T>()
            .ObserveOn(scheduler)
            .SubscribeOn(scheduler)
            .Subscribe(
                async @event =>
                {
                    try
                    {
                        await handler(@event);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error handling event of type {EventType}", typeof(T));
                        throw new ReactiveSubscriptionException("Error handling event", ex);
                    }
                },
                async ex =>
                {
                    if (error is not null)
                        await error(ex);
                    logger.LogError(ex, "Error in event stream for {EventType}", typeof(T));
                },
                async () =>
                {
                    if (complete is not null)
                        await complete();
                    logger.LogInformation("Completed in event stream for {EventType}", typeof(T));
                }
            );
        // Wrap subscription to ensure proper cleanup
        IDisposable managedSubscription = CreateManagedSubscription(subscription);
        return managedSubscription;
    }

    public IObservable<T> Observe<T>()
    {
        return subject.OfType<T>().ObserveOn(scheduler);
    }

    public IDisposable Subscribe<T, TTransform>(Func<IObservable<T>, IObservable<TTransform>> transform)
    {
        IObservable<T> observable = Observe<T>();
        IObservable<TTransform> transformed = transform(observable)
            .SubscribeOn(scheduler);
        IDisposable managedSubscription = CreateManagedSubscription(transformed.Subscribe());
        return managedSubscription;
    }

    private IDisposable CreateManagedSubscription(IDisposable subscription)
    {
        lock (_lock)
        {
            subscriptions.Add(subscription);
        }

        return Disposable.Create(() =>
        {
            lock (_lock)
            {
                subscriptions.Remove(subscription);
            }
            subscription.Dispose();
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.Exchange(ref isStarting, 1) == 1)
        {
            logger.LogWarning("Start was called while service is already starting");
            await isStarted.Task.WaitAsync(cancellationToken);
            return;
        }

        try
        {
            logger.LogInformation("Starting ReactiveSubscriptions service");
            isStarted.TrySetResult(true);
        }
        finally
        {
            Interlocked.Exchange(ref isStarting, 0);
        }
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        TaskCompletionSource<bool> currentSource = isStarted;

        if (!currentSource.Task.IsCompletedSuccessfully)
        {
            logger.LogWarning("Stop was called while service is not running");
            return Task.CompletedTask;
        }

        logger.LogInformation("Stopping ReactiveSubscriptions service");

        var newSource = new TaskCompletionSource<bool>();
        if (Interlocked.CompareExchange(ref isStarted, newSource, currentSource) == currentSource)
        {
            currentSource.TrySetCanceled();
        }

        return Task.CompletedTask;
    }

    public async Task RestartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Restarting ReactiveSubscriptions service");
        await StopAsync(cancellationToken);
        await StartAsync(cancellationToken);
    }

    public void Dispose()
    {
        logger.LogInformation("Disposing ReactiveSubscriptions service");

        isStarted.TrySetCanceled();

        lock (_lock)
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }
            subscriptions.Clear();
        }

        subject.OnCompleted();
    }
}
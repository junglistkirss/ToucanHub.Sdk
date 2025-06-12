using Microsoft.Extensions.Logging;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Toucan.Sdk.Reactive;

internal class ManagedReactive(ILogger<ManagedReactive> logger, IReactiveLauncherSchedulerProvider? schedulerProvider = null) : IReactiveDispatcher, IReactiveManager, IDisposable
{

    private readonly Subject<object> subject = new();
    private readonly CompositeDisposable subscriptions = [];
    private readonly IScheduler scheduler = schedulerProvider?.GetScheduler() ?? TaskPoolScheduler.Default;
    private readonly Lock _lock = new();

    public void Dispose()
    {
        logger.LogInformation("Disposing ManagedReactive service");

        subscriptions.Dispose();

        subject.OnCompleted();
    }

    public void Complete()
    {
        subject.OnCompleted();
    }

    public void Publish<T>(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        subject.OnNext(value);
    }

    public IDisposable Subscribe<T>(Action<T> handler, Action<Exception>? error = null, Action? complete = null)
    {
        ArgumentNullException.ThrowIfNull(handler);

        IDisposable subscription = subject
            .OfType<T>()
            .ObserveOn(scheduler)
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
                        subject.OnError(new ReactiveSubscriptionException("Error handling event", ex));
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

    public IDisposable Subscribe<T>(Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null)
    {
        ArgumentNullException.ThrowIfNull(handler);

        IDisposable subscription = subject
            .OfType<T>()
            .ObserveOn(scheduler)
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
                        subject.OnError(new ReactiveSubscriptionException("Error handling event", ex));
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
                        subject.OnError(new ReactiveSubscriptionException("Error handling event", ex));
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

    public void Throw(Exception value)
    {
        ArgumentNullException.ThrowIfNull(value);
        subject.OnError(value);
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Numerics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Toucan.Sdk.Reactive;

public interface IReactiveLauncherSchedulerProvider
{
    IScheduler GetScheduler();
}


public interface IReactiveManagedDispatcher
{
    void Publish<T>(Guid serviceId, T value);
    void Complete(Guid serviceId);
}

public interface IReactiveManagedSubscriber
{
    IDisposable Subscribe<T>(Guid serviceId, Action<T> handler, Action<Exception>? error = null, Action? complete = null);
    IDisposable Subscribe<T>(Guid serviceId, Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null);
    IDisposable Subscribe<T>(Guid serviceId, Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null);
}


public interface IReactiveLauncher
{
    Task<bool> WaitForStart(CancellationToken cancellationToken = default);

    Guid Initialize();
    void Kill(Guid serviceId);
    IObservable<ChildServiceInfo> Observe();
}

internal interface IReactiveDispatcher
{
    void Publish<T>(T value);
    void Complete();
}

public enum ManagedServiceState
{
    Started,
    Stopped,
}

public record class ChildServiceInfo(Guid Id, ManagedServiceState State);

internal interface IReactiveManager
{
    IDisposable Subscribe<T>(Action<T> handler, Action<Exception>? error = null, Action? complete = null);
    IDisposable Subscribe<T>(Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null);
    IDisposable Subscribe<T>(Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null);
}

internal class ManagedReactive(ILogger<ManagedReactive> logger, IReactiveLauncherSchedulerProvider? schedulerProvider = null) : IReactiveDispatcher, IReactiveManager, IDisposable
{

    private readonly Subject<object> subject = new();
    private readonly CompositeDisposable subscriptions = new();
    private readonly IScheduler scheduler = schedulerProvider?.GetScheduler() ?? TaskPoolScheduler.Default;
    private readonly object _lock = new();

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
}

internal class SharedReactive(ILogger<SharedReactive> logger, IServiceProvider provider) : IHostedService, IReactiveLauncher, IReactiveManagedDispatcher, IReactiveManagedSubscriber
{
    private readonly Subject<ChildServiceInfo> subject = new();
    private readonly CompositeDisposable subscriptions = new();
    private readonly ConcurrentDictionary<Guid, ManagedReactive> services = new();
    private readonly object _lock = new();
    private TaskCompletionSource<bool> isStarted = new();
    private int isStarting;
    public Task<bool> WaitForStart(CancellationToken cancellationToken = default)
    {
        return isStarted.Task.WaitAsync(cancellationToken);
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
            foreach (ManagedReactive item in services.Values)
                item.Dispose();
            subscriptions.Dispose();
        }

        subject.OnCompleted();
    }

    public Guid Initialize()
    {
        if (!isStarted.Task.IsCompletedSuccessfully)
        {
            logger.LogError("Service is not running");
            throw new InvalidOperationException("Service is not running");
        }

        Guid uid = Guid.NewGuid();
        ManagedReactive managed = provider.GetRequiredService<ManagedReactive>();
        if (services.TryAdd(uid, managed))
        {
            ChildServiceInfo info = new(uid, ManagedServiceState.Started);
            subject.OnNext(info);
            logger.LogDebug($"Service {info} is running");
            return uid;
        }
        return Guid.Empty;
    }

    public void Kill(Guid serviceId)
    {
        if (services.TryRemove(serviceId, out var service))
        {
            service.Dispose();
            ChildServiceInfo info = new(serviceId, ManagedServiceState.Stopped);
            subject.OnNext(info);
            logger.LogDebug($"Service {info} is killed");
        }
    }

    public IObservable<ChildServiceInfo> Observe()
    {
        return subject.AsObservable();
    }

    public IDisposable Subscribe<T>(Guid serviceId, Action<T> handler, Action<Exception>? error = null, Action? complete = null)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service) )
        {
            return service.Subscribe(handler, error, complete); 
        }
        return Disposable.Empty;
    }

    public IDisposable Subscribe<T>(Guid serviceId, Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Subscribe(handler, error, complete);
        }
        return Disposable.Empty;
    }

    public IDisposable Subscribe<T>(Guid serviceId, Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Subscribe(handler, error, complete);
        }
        return Disposable.Empty;
    }

    public void Publish<T>(Guid serviceId, T value)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Publish(value);
        }
    }

    public void Complete(Guid serviceId)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Complete();
        }
    }
}
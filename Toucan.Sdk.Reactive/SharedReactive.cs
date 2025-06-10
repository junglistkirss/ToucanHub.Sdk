using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Toucan.Sdk.Reactive;

public delegate TServiceId GenerateServiceId<TServiceId>()
    where TServiceId : IEquatable<TServiceId>;

internal class SharedReactive<TServiceId>(ILogger<SharedReactive<TServiceId>> logger, GenerateServiceId<TServiceId> generateServiceId, IServiceProvider provider) : IHostedService, IReactiveLauncher<TServiceId>, IReactiveManagedDispatcher<TServiceId>, IReactiveManagedSubscriber<TServiceId>
    where TServiceId : IEquatable<TServiceId>
{
    private readonly Subject<ChildServiceInfo<TServiceId>> subject = new();
    private readonly CompositeDisposable subscriptions = new();
    private readonly ConcurrentDictionary<TServiceId, ManagedReactive> services = new();
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

    public TServiceId Initialize()
    {
        if (!isStarted.Task.IsCompletedSuccessfully)
        {
            logger.LogError("Service is not running");
            throw new InvalidOperationException("Service is not running");
        }

        TServiceId uid = generateServiceId();
        ManagedReactive managed = provider.GetRequiredService<ManagedReactive>();
        if (services.TryAdd(uid, managed))
        {
            ChildServiceInfo<TServiceId> info = new(uid, ManagedServiceState.Started);
            subject.OnNext(info);
            logger.LogDebug($"Service {info} is running");
            return uid;
        }
        throw new InvalidOperationException($"Service with ID {uid} is already initialized");
    }
    public void Initialize(TServiceId uid)
    {
        if (!isStarted.Task.IsCompletedSuccessfully)
        {
            logger.LogError("Service is not running");
            throw new InvalidOperationException("Service is not running");
        }

        ManagedReactive managed = provider.GetRequiredService<ManagedReactive>();
        if (services.TryAdd(uid, managed))
        {
            ChildServiceInfo<TServiceId> info = new(uid, ManagedServiceState.Started);
            subject.OnNext(info);
            logger.LogDebug($"Service {info} is running");
        }else{
        throw new InvalidOperationException($"Service with ID {uid} is already initialized");
        }
    }

    public void Kill(TServiceId serviceId)
    {
        if (services.TryRemove(serviceId, out var service))
        {
            service.Dispose();
            ChildServiceInfo<TServiceId> info = new(serviceId, ManagedServiceState.Stopped);
            subject.OnNext(info);
            logger.LogDebug($"Service {info} is killed");
        }
    }

    public IObservable<ChildServiceInfo<TServiceId>> Observe()
    {
        return subject.AsObservable();
    }

    public IDisposable Subscribe<T>(TServiceId serviceId, Action<T> handler, Action<Exception>? error = null, Action? complete = null)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Subscribe(handler, error, complete);
        }
        return Disposable.Empty;
    }

    public IDisposable Subscribe<T>(TServiceId serviceId, Func<T, ValueTask> handler, Func<Exception, ValueTask>? error = null, Func<ValueTask>? complete = null)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Subscribe(handler, error, complete);
        }
        return Disposable.Empty;
    }

    public IDisposable Subscribe<T>(TServiceId serviceId, Func<T, Task> handler, Func<Exception, Task>? error = null, Func<Task>? complete = null)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Subscribe(handler, error, complete);
        }
        return Disposable.Empty;
    }

    public void Publish<T>(TServiceId serviceId, T value)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Publish(value);
        }
    }

    public void Complete(TServiceId serviceId)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Complete();
        }
    }

    public void Throw(TServiceId serviceId, Exception exception)
    {
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Throw(exception);
        }
    }
}
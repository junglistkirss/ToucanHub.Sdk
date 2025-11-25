using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ToucanHub.Sdk.Reactive;

public delegate TServiceId GenerateServiceId<TServiceId>()
    where TServiceId : struct;

internal class SharedReactive<TServiceId>(ILogger<SharedReactive<TServiceId>> logger, GenerateServiceId<TServiceId> generateServiceId, IServiceProvider provider) : IHostedService, IReactiveLauncher<TServiceId>, IReactiveManagedDispatcher<TServiceId>, IReactiveManagedSubscriber<TServiceId>
    where TServiceId : struct
{
    private readonly Subject<ChildServiceInfo<TServiceId>> subject = new();
    private readonly CompositeDisposable subscriptions = [];
    private readonly ConcurrentDictionary<TServiceId, ManagedReactive> services = new();
    private readonly Lock _lock = new();
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
            currentSource.TrySetCanceled(cancellationToken);
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
        EnsureStarted();

        TServiceId uid = generateServiceId();
        ManagedReactive managed = provider.GetRequiredService<ManagedReactive>();
        if (services.TryAdd(uid, managed))
        {
            ChildServiceInfo<TServiceId> info = new(uid, ManagedServiceState.Started);
            subject.OnNext(info);
            logger.LogDebug("Service {msg} is running", info);
            return uid;
        }
        throw new InvalidOperationException($"Service with ID {uid} is already initialized");
    }

    private void EnsureStarted()
    {
        if (!isStarted.Task.IsCompletedSuccessfully)
        {
            logger.LogError("Service is not running");
            throw new InvalidOperationException("Service is not running");
        }
    }

    public void Initialize(TServiceId uid)
    {
        EnsureStarted();


        ManagedReactive managed = provider.GetRequiredService<ManagedReactive>();
        if (services.TryAdd(uid, managed))
        {
            ChildServiceInfo<TServiceId> info = new(uid, ManagedServiceState.Started);
            subject.OnNext(info);
            logger.LogDebug("Service {msg} is running", info);
        }
        else
        {
            throw new InvalidOperationException($"Service with ID {uid} is already initialized");
        }
    }

    public void Kill(TServiceId serviceId)
    {
        EnsureStarted();

        if (services.TryRemove(serviceId, out var service))
        {
            service.Dispose();
            ChildServiceInfo<TServiceId> info = new(serviceId, ManagedServiceState.Stopped);
            subject.OnNext(info);
            logger.LogDebug("Service {msg} is running", info);
        }
    }

    public IObservable<ChildServiceInfo<TServiceId>> Observe()
    {
        EnsureStarted();

        return subject.AsObservable();
    }


    public IObservable<T> Observe<T>(TServiceId serviceId)
    {
        EnsureStarted();
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Observe<T>();
        }
        return Observable.Empty<T>();
    }

    public void Publish<T>(TServiceId serviceId, T value)
    {
        EnsureStarted();
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Publish(value);
        }
    }

    public void Complete(TServiceId serviceId)
    {
        EnsureStarted();
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Complete();
        }
    }

    public void Throw(TServiceId serviceId, Exception exception)
    {
        EnsureStarted();
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            service.Throw(exception);
        }
    }

    public IDisposable Register(TServiceId serviceId, IDisposable disposable)
    {
        EnsureStarted();
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Register(disposable);
        }
        return Disposable.Empty;
    }

    public IDisposable Register<T>(TServiceId serviceId, Func<IObservable<T>, IDisposable> configure)
    {
        EnsureStarted();
        if (services.TryGetValue(serviceId, out ManagedReactive? service))
        {
            return service.Register(configure(service.Observe<T>()));
        }
        return Disposable.Empty;
    }
}
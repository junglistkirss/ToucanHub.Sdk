using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Toucan.Sdk.Reactive;



public interface ISubscrptionsSchedulerProvider
{
    IScheduler Scheduler { get; }
}

internal class ReactiveSubscriptions(ILogger<ReactiveSubscriptions> logger, ISubscrptionsSchedulerProvider? subscrptionsSchedulerProvider = null) : IHostedService, ISubscriptionDispatcher, ISubscriptionManager, IDisposable
{
    private readonly Subject<object> subject = new();
    private readonly IScheduler scheduler = subscrptionsSchedulerProvider?.Scheduler ?? TaskPoolScheduler.Default;
    private readonly CompositeDisposable subscriptions = [];
    private readonly Lock _lock = new();
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

    public IObservable<T> Observe<T>()
    {
        return subject.OfType<T>().ObserveOn(scheduler);
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
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }
            subscriptions.Clear();
        }

        subject.OnCompleted();
    }

    public void Throw(Exception value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (isStarted.Task.IsCompletedSuccessfully)
        {
            subject.OnError(value);
        }
        else
        {
            logger.LogWarning("Attempted to throw error when service is not started");
        }
    }

    public IDisposable Register(IDisposable disposable)
    {
        return CreateManagedSubscription(disposable);
    }

    public IDisposable Register<T>(Func<IObservable<T>, IDisposable> configure)
    {
        return CreateManagedSubscription(configure(Observe<T>()));
    }
}
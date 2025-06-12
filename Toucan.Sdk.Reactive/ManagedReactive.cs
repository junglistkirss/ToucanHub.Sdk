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

    public void Throw(Exception value)
    {
        ArgumentNullException.ThrowIfNull(value);
        subject.OnError(value);
    }

    public IObservable<T> Observe<T>()
    {
        return subject
           .OfType<T>()
           .ObserveOn(scheduler);
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

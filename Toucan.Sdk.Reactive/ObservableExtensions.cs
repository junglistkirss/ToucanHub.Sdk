using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Toucan.Sdk.Reactive;

public static class ObservableExtensions
{

    public static IObservable<T> SafeSubscribe<T>(
        this IObservable<T> source,
        Action<T> onNext,
        Action<Exception>? onException = null)
    {
        return Observable.Create<T>(observer =>
        {
            return source.Subscribe(
                value =>
                {
                    try
                    {
                        onNext(value);
                        observer.OnNext(value);
                    }
                    catch (Exception ex)
                    {
                        onException?.Invoke(ex);
                        // Continue le flux sans propager l'exception
                        observer.OnNext(value);
                    }
                },
                observer.OnError,
                observer.OnCompleted
            );
        });
    }

    public static IObservable<T> SafeSubscribeWithFilter<T>(
        this IObservable<T> source,
        Action<T> onNext,
        Action<Exception, T>? onException = null)
    {
        return Observable.Create<T>(observer =>
        {
            return source.Subscribe(
                value =>
                {
                    try
                    {
                        onNext(value);
                        observer.OnNext(value);
                    }
                    catch (Exception ex)
                    {
                        onException?.Invoke(ex, value);
                        // L'élément est filtré (pas de OnNext)
                    }
                },
                observer.OnError,
                observer.OnCompleted
            );
        });
    }
    //public static IObservable<T> RepeatWhen<T>(
    //    this IObservable<T> source,
    //    Func<IObservable<Unit>, IObservable<Unit>> handler)
    //{
    //    return Observable.Defer(() =>
    //    {
    //        var completions = new Subject<Unit>();
    //        var repeatSignal = handler(completions.AsObservable());

    //        return source
    //            .Do(onCompleted: () => completions.OnNext(Unit.Default))
    //            .TakeUntil(repeatSignal.IgnoreElements())
    //            .Repeat();
    //    });
    //}
    public static IObservable<T> WhereNotNull<T>(this IObservable<T> source)
       where T : class
    {
        return source.Where(x => x != null);
    }
    public static IObservable<T> WhereNotNull<T>(this IObservable<T?> source)
        where T : struct
    {
        return source.Where(x => x.HasValue).Select(x => x!.Value);
    }

    public static IObservable<T> TimeoutWith<T>(
        this IObservable<T> source,
        TimeSpan timeout,
        IObservable<T> fallback,
        IScheduler? scheduler = null)
    {
        scheduler ??= DefaultScheduler.Instance;

        return source.Timeout(timeout, fallback, scheduler);
    }
    public static IObservable<T> RetryWithBackoff<T>(
        this IObservable<T> source,
        int maxRetries = 3,
        TimeSpan? initialDelay = null,
        TimeSpan? maxDelay = null,
        double backoffMultiplier = 2.0,
        IScheduler? scheduler = null)
    {
        initialDelay ??= TimeSpan.FromSeconds(1);
        maxDelay ??= TimeSpan.FromMinutes(1);
        scheduler ??= DefaultScheduler.Instance;

        return Observable.Defer(() =>
        {
            var attempt = 0;
            return source.RetryWhen(errors =>
                errors.SelectMany(error =>
                {
                    if (++attempt > maxRetries)
                    {
                        return Observable.Throw<Unit>(error);
                    }

                    var delay = TimeSpan.FromMilliseconds(
                        Math.Min(
                            initialDelay.Value.TotalMilliseconds * Math.Pow(backoffMultiplier, attempt - 1),
                            maxDelay.Value.TotalMilliseconds
                        )
                    );

                    return Observable.Timer(delay, scheduler).Select(_ => Unit.Default);
                })
            );
        });
    }
    public static IObservable<IList<T>> BufferWithTimeOrCount<T>(
        this IObservable<T> source,
        TimeSpan timeSpan,
        int count,
        IScheduler? scheduler = null)
    {
        scheduler ??= DefaultScheduler.Instance;

        return Observable.Create<IList<T>>(observer =>
        {
            var buffer = new List<T>();
            var gate = new object();
            var timerDisposable = new SerialDisposable();

            void FlushBuffer()
            {
                lock (gate)
                {
                    if (buffer.Count > 0)
                    {
                        observer.OnNext([.. buffer]);
                        buffer.Clear();
                    }
                    // Reset timer
                    timerDisposable.Disposable = scheduler.Schedule(timeSpan, FlushBuffer);
                }
            }

            // Start initial timer
            timerDisposable.Disposable = scheduler.Schedule(timeSpan, FlushBuffer);

            var sourceSubscription = source.Subscribe(
                onNext: item =>
                {
                    lock (gate)
                    {
                        buffer.Add(item);
                        if (buffer.Count >= count)
                        {
                            observer.OnNext([.. buffer]);
                            buffer.Clear();
                            // Reset timer
                            timerDisposable.Disposable = scheduler.Schedule(timeSpan, FlushBuffer);
                        }
                    }
                },
                onError: observer.OnError,
                onCompleted: () =>
                {
                    lock (gate)
                    {
                        if (buffer.Count > 0)
                        {
                            observer.OnNext([.. buffer]);
                        }
                    }
                    observer.OnCompleted();
                }
            );

            return new CompositeDisposable(sourceSubscription, timerDisposable);
        });
    }
    public static IObservable<IList<T>> RollingBuffer<T>(
        this IObservable<T> source,
        int windowSize)
    {
        if (windowSize <= 0)
            throw new ArgumentException("Window size must be positive", nameof(windowSize));

        return Observable.Create<IList<T>>(observer =>
        {
            var buffer = new Queue<T>();
            var gate = new object();

            return source.Subscribe(
                onNext: item =>
                {
                    lock (gate)
                    {
                        buffer.Enqueue(item);

                        while (buffer.Count > windowSize)
                        {
                            buffer.Dequeue();
                        }

                        observer.OnNext([.. buffer]);
                    }
                },
                onError: observer.OnError,
                onCompleted: observer.OnCompleted
            );
        });
    }
    public static IObservable<T> DistinctBy<T, TKey>(
        this IObservable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        comparer ??= EqualityComparer<TKey>.Default;

        return Observable.Create<T>(observer =>
        {
            var seenKeys = new HashSet<TKey>(comparer);
            var gate = new object();

            return source.Subscribe(
                onNext: item =>
                {
                    lock (gate)
                    {
                        var key = keySelector(item);
                        if (seenKeys.Add(key))
                        {
                            observer.OnNext(item);
                        }
                    }
                },
                onError: observer.OnError,
                onCompleted: observer.OnCompleted
            );
        });
    }
    public static IObservable<KeyValuePair<TKey, TAccumulate>> AggregateBy<T, TKey, TAccumulate>(
        this IObservable<T> source,
        Func<T, TKey> keySelector,
        TAccumulate seed,
        Func<TAccumulate, T, TAccumulate> accumulator)
        where TKey : notnull
    {
        return Observable.Create<KeyValuePair<TKey, TAccumulate>>(observer =>
        {
            var aggregates = new Dictionary<TKey, TAccumulate>();
            var gate = new object();

            return source.Subscribe(
                onNext: item =>
                {
                    lock (gate)
                    {
                        var key = keySelector(item);

                        if (!aggregates.TryGetValue(key, out TAccumulate? value))
                        {
                            value = seed;
                            aggregates[key] = value;
                        }

                        aggregates[key] = accumulator(value, item);
                        observer.OnNext(new KeyValuePair<TKey, TAccumulate>(key, aggregates[key]));
                    }
                },
                onError: observer.OnError,
                onCompleted: observer.OnCompleted
            );
        });
    }
    public static IObservable<T> Spy<T>(
        this IObservable<T> source,
        string? tag = null,
        Action<string>? logger = null)
    {
        tag ??= typeof(T).Name;
        logger ??= (msg => System.Diagnostics.Debug.WriteLine(msg));

        return Observable.Create<T>(observer =>
        {
            logger($"[{tag}] Subscribing at {DateTime.Now:HH:mm:ss.fff}");

            return source.Subscribe(
                onNext: value =>
                {
                    logger($"[{tag}] OnNext({value}) at {DateTime.Now:HH:mm:ss.fff}");
                    observer.OnNext(value);
                },
                onError: error =>
                {
                    logger($"[{tag}] OnError({error.Message}) at {DateTime.Now:HH:mm:ss.fff}");
                    observer.OnError(error);
                },
                onCompleted: () =>
                {
                    logger($"[{tag}] OnCompleted at {DateTime.Now:HH:mm:ss.fff}");
                    observer.OnCompleted();
                }
            );
        })
        .Finally(() => logger($"[{tag}] Finally at {DateTime.Now:HH:mm:ss.fff}"));
    }
    public static IObservable<T> WhereAsync<T>(
        this IObservable<T> source,
        Func<T, Task<bool>> asyncPredicate,
        int maxConcurrency = 1)
    {
        return source
            .Select(item => Observable.FromAsync(() => asyncPredicate(item))
                .Select(result => new { Item = item, Include = result }))
            .Merge(maxConcurrency)
            .Where(x => x.Include)
            .Select(x => x.Item);
    }
    public static IObservable<T> Pausable<T>(this IObservable<T> source, IObservable<bool> pauseSignal, bool paused = false)
    {
        return pauseSignal
            .StartWith(paused) // Commencer en mode non-pausé
            .DistinctUntilChanged()
            .Select(isPaused => isPaused ? Observable.Empty<T>() : source)
            .Switch();
    }

    public static BufferedPausableObservable<T> BufferedPausable<T>(this IObservable<T> source)
    {
        return new BufferedPausableObservable<T>(source);
    }
    public static PausableObservable<T> Pausable<T>(this IObservable<T> source)
    {
        return new PausableObservable<T>(source);
    }
}


using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Toucan.Sdk.Reactive;

public sealed class PausableObservable<T> : IObservable<T>
{
    private readonly BehaviorSubject<bool> _isRunning;
    private readonly IObservable<T> _pausable;

    public PausableObservable(IObservable<T> source, bool running = true)
    {
        _isRunning = new BehaviorSubject<bool>(running);
        _pausable = _isRunning
            .DistinctUntilChanged()
            .Select(isPaused => isPaused ? Observable.Empty<T>() : source)
            .Switch();
    }

    public void Pause() => _isRunning.OnNext(false);
    public void Resume() => _isRunning.OnNext(true);

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return _pausable.Subscribe(observer);
    }
}


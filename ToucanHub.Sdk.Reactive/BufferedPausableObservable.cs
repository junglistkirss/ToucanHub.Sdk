using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ToucanHub.Sdk.Reactive;

public sealed class BufferedPausableObservable<T> : IObservable<T>
{
    private readonly BehaviorSubject<bool> _isRunning;
    private readonly IObservable<T> _pausable;

    public BufferedPausableObservable(IObservable<T> source, bool running = true)
    {
        _isRunning = new BehaviorSubject<bool>(running);
        _pausable = source
            .Buffer(_isRunning.DistinctUntilChanged().Where(running => running))
            .Where(buffer => buffer.Count > 0)
            .SelectMany(buffer => buffer);
    }

    public void Pause() => _isRunning.OnNext(false);
    public void Resume() => _isRunning.OnNext(true);

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return _pausable.Subscribe(observer);
    }
}


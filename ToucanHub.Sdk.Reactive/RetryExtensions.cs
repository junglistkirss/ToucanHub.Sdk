using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ToucanHub.Sdk.Reactive;

public static class RetryExtensions
{
    public static IObservable<T> RetryWhen<T>(
        this IObservable<T> source,
        Func<IObservable<Exception>, IObservable<Unit>> handler)
    {
        return Observable.Defer(() =>
        {
            var errors = new Subject<Exception>();
            var retrySignal = handler(errors.AsObservable());

            return source
                .Catch<T, Exception>(ex =>
                {
                    errors.OnNext(ex);
                    return Observable.Empty<T>();
                })
                .RepeatWhen(_ => retrySignal);
        });
    }
}


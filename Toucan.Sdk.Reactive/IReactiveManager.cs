namespace Toucan.Sdk.Reactive;

internal interface IReactiveManager : IDisposableSubscriptionManager
{
    IObservable<T> Observe<T>();
}

namespace ToucanHub.Sdk.Reactive;

internal interface IReactiveManager : IDisposableSubscriptionManager
{
    IObservable<T> Observe<T>();
}

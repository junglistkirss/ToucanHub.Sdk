namespace Toucan.Sdk.Reactive;

public interface ISubscriptionDispatcher
{
    void Publish<T>(T value);
}


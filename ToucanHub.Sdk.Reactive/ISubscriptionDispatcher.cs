namespace ToucanHub.Sdk.Reactive;

public interface ISubscriptionDispatcher
{
    void Publish<T>(T value);
    void Throw(Exception value);
    void Complete();
}



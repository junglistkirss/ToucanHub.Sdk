namespace Toucan.Sdk.Reactive;

internal interface IReactiveDispatcher
{
    void Publish<T>(T value);
    void Throw(Exception value);
    void Complete();
}

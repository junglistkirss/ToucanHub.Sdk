namespace Toucan.Sdk.Core.Aggregates;


public interface IEventSourced<T>
{
    void ApplyMutation(T e);
}
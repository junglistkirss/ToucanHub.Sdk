namespace ToucanHub.Sdk.Contracts.Surrogates;

public interface ITarget<TTarget>
{
    TTarget ToTarget();
}
public interface ITarget<T, TArgs>
{
    T ToTarget(TArgs args);
}

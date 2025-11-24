namespace ToucanHub.Sdk.Contracts.Surrogates;

public interface IProxy<TSource, TTarget>
{
    static abstract TTarget Map(TSource source);
}
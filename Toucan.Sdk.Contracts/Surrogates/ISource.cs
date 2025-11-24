namespace Toucan.Sdk.Contracts.Surrogates;

public interface ISource<TSource>
{
    void FromSource(TSource source);
}

public interface ISource<TSource, TArgs>
{
    void FromSource(TSource source, TArgs args);
}

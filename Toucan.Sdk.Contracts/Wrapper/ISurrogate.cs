namespace Toucan.Sdk.Contracts.Wrapper;


public delegate TOut Projector<TIn, TOut>(TIn input);
public delegate TOut Projector<TIn, TOut, TArgs>(TIn input, TArgs args);

public interface ITarget<T, TArgs>
{
    T ToTarget(TArgs args);
}


public interface ISource<TSource, TArgs>
{
    void FromSource(TSource source, TArgs args);
}

public interface ISource<TSource>
{
    void FromSource(TSource source);
}

public interface ITarget<TTarget>
{
    TTarget ToTarget();
}


public interface ISurrogate<T> : ISource<T>, ITarget<T> { }
public interface ISurrogate<T, TArgs> : ISource<T, TArgs>, ITarget<T, TArgs> { }

public interface IProxy<TSource, TTarget>
{
    static abstract TTarget Map(TSource source);
}
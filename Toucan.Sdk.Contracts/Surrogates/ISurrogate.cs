namespace Toucan.Sdk.Contracts.Surrogates;

public interface ISurrogate<T> : ISource<T>, ITarget<T> { }
public interface ISurrogate<T, TArgs> : ISource<T, TArgs>, ITarget<T, TArgs> { }

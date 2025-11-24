namespace Toucan.Sdk.Contracts.Surrogates;

public delegate TOut Projector<TIn, TOut>(TIn input);
public delegate TOut Projector<TIn, TOut, TArgs>(TIn input, TArgs args);


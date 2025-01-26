namespace Toucan.Sdk.Interpreter;

public readonly record struct EvaluationResult<T>
{
    public Exception? Exception { get; init; }

    public T? Response { get; init; }

}

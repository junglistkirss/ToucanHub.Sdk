namespace Toucan.Sdk.Interpreter;

public readonly record struct RunResult
{
    public Exception? Exception { get; init; }

}

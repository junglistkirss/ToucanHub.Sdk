namespace Toucan.Sdk.Interpreter;

public readonly record struct RunRequest
{
    public object? Context { get; init; }
    public string Code { get; init; }
    public CancellationToken CancellationToken { get; init; }
}

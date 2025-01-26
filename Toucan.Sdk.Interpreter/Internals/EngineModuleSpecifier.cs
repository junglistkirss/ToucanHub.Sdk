namespace Toucan.Sdk.Interpreter.Internals;

internal sealed class EngineModuleSpecifier : IEngineModule
{
    public string Code { get; init; } = default!;
}
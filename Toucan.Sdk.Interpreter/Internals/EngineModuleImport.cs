namespace Toucan.Sdk.Interpreter.Internals;

internal sealed class EngineModuleImport : IEngineModule
{
    public Uri Path { get; init; } = default!;
}

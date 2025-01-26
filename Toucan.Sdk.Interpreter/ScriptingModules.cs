using Toucan.Sdk.Interpreter.Internals;

namespace Toucan.Sdk.Interpreter;

public static class ScriptingModules
{
    public static IEngineModule FromPath(Uri path)
    {
        return new EngineModuleImport
        {
            Path = path,
        };
    }

    public static IEngineModule FromCode(string code)
    {
        return new EngineModuleSpecifier
        {
            Code = code,
        };
    }
}
using System.Diagnostics;

namespace Toucan.Sdk.Interpreter;

public static class InterpreterTelemetry
{
    public const string SourceName = "interpreter";

    private static readonly ActivitySource source = new(SourceName);

    internal static Activity? Start(string name) => source.StartActivity(name);
}

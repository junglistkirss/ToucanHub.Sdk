using System.Globalization;

namespace Toucan.Sdk.Interpreter;

public sealed record class InterpreterOptions
{
    public static readonly InterpreterOptions Default = new ();

    public bool StrictMode { get; init; }
    public bool DebugMode { get; init; }
    public bool DisableStringCompilation { get; init; } = true;

    /// <summary>
    /// Default is 4GB
    /// </summary>
    public long LimitMemory { get; init; } = 4_000_000_000;

    public TimeSpan RegexTimeoutInterval { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan TimeoutInterval { get; init; } = TimeSpan.FromSeconds(5);
    public Dictionary<string, IEngineModule> ScriptModules { get; init; } = [];

    /// <summary>
    /// Whether to register require function to engine which will delegate to module loader, defaults to false.
    /// </summary>
    public bool RegisterRequire { get; init; } = false;
    /// <summary>
    /// Whether calling 'eval' with custom code and function constructors taking function code as string is allowed.
    /// Defaults to true.
    /// </summary>
    /// <remarks>
    /// https://tc39.es/ecma262/#sec-hostensurecancompilestrings
    /// </remarks>
    public bool StringCompilationAllowed { get; init; } = false;

    public string? ModulesBasePath { get; init; } = null;

    public CultureInfo Culture { get; init; } = CultureInfo.CurrentCulture;
    public TimeZoneInfo TimeZoneInfo { get; init; } = TimeZoneInfo.Local;
}

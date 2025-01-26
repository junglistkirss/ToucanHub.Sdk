using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toucan.Sdk.Interpreter.Internals;

namespace Toucan.Sdk.Interpreter;

public static class InterpreterModule
{
    public static IServiceCollection ConfigureInterpreter(this IServiceCollection services, InterpreterOptions options, IEngineExchange engineExchange)
    {
        services.TryAddSingleton<IModuleParser, ModuleParser>();
        services.TryAddSingleton<IScriptParser, ScriptParser>();
        services.AddMemoryCache(cfg =>
        {
            cfg.TrackStatistics = true;
        });

        services.AddScoped<IInterpreter, JintInterpreter>();

        return services
            .AddSingleton(engineExchange)
            .AddSingleton(options);
    }
}

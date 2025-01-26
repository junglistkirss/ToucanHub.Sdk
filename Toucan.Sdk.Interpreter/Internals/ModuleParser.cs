using Jint;
using Microsoft.Extensions.Caching.Memory;
using JsModule = Acornima.Ast.Module;
namespace Toucan.Sdk.Interpreter.Internals;

internal sealed class ModuleParser(IMemoryCache cache) : IModuleParser
{
    public Prepared<JsModule> GetModule(string key, IEngineModule engineModule)
    {
        using (InterpreterTelemetry.Start("get_module"))
        {
            string moduleKey = $"module::{key}";
            if (!cache.TryGetValue(moduleKey, out Prepared<JsModule>? module))
            {
                switch (engineModule)
                {
                    case var _ when engineModule is EngineModuleImport imp:
                        string code = File.ReadAllText(imp.Path.ToString());
                        module = Engine.PrepareModule(code);
                        break;
                    case var _ when engineModule is EngineModuleSpecifier spec:
                        module = Engine.PrepareModule(spec.Code);
                        break;
                    default:
                        throw new InvalidCastException("Unknown module type, module parser must be overriden");
                }
                cache.Set(moduleKey, module, DateTimeOffset.Now.AddHours(1));
            }
            return module!.Value;
        }
    }
}

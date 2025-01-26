using Jint;
using JsModule = Acornima.Ast.Module;
namespace Toucan.Sdk.Interpreter;

public interface IModuleParser
{
    Prepared<JsModule> GetModule(string key, IEngineModule engineModule);
}

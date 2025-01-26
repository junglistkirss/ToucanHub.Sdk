using Jint;
using JsScript = Acornima.Ast.Script;
namespace Toucan.Sdk.Interpreter;

public interface IScriptParser
{
    Prepared<JsScript> GetScript(string code);
}

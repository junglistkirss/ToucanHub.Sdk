using Jint;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using JsScript = Acornima.Ast.Script;
namespace Toucan.Sdk.Interpreter.Internals;

internal sealed class ScriptParser(IMemoryCache cache) : IScriptParser
{
    public Prepared<JsScript> GetScript(string code)
    {
        using (InterpreterTelemetry.Start("get_script"))
        {
            string key = $"script::{SHA1.HashData(Encoding.UTF8.GetBytes(code))}";
            if (!cache.TryGetValue(key, out Prepared<JsScript>? script))
            {
                script = Engine.PrepareScript(code);
                cache.Set(key, script, DateTimeOffset.Now.AddMinutes(10));
            }
            if (script.HasValue)
                return script.Value;
            throw new InvalidOperationException("Missing script");
        }
    }
}

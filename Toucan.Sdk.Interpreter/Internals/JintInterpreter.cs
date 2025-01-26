using Jint;
using Jint.Constraints;
using Jint.Native;
using Microsoft.Extensions.Logging;
using Toucan.Sdk.Interpreter.Exceptions;
using JsModule = Acornima.Ast.Module;
using JsScript = Acornima.Ast.Script;
namespace Toucan.Sdk.Interpreter.Internals;

internal sealed class JintInterpreter : IInterpreter, IDisposable
{
    private readonly IModuleParser moduleParser;
    private readonly IScriptParser scriptParser;
    private readonly ILogger<IInterpreter> logger;
    private readonly InterpreterOptions options;
    private readonly IEngineExchange exchange;
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private readonly Lazy<Engine> instance;
    public JintInterpreter(
        IModuleParser moduleParser,
        IScriptParser scriptParser,
        ILogger<IInterpreter> logger,
        InterpreterOptions options,
        IEngineExchange exchange)
    {

        this.moduleParser = moduleParser;
        this.scriptParser = scriptParser;
        this.logger = logger;
        this.options = options;
        this.exchange = exchange;
        instance = new Lazy<Engine>(Initialize);
    }

    private Engine Initialize()
    {
        using (InterpreterTelemetry.Start("initialize"))
        {
            Engine engine = new Engine(cfg =>
            {
                cfg.Strict(options.StrictMode);
                cfg.DebugMode(options.DebugMode);
                cfg.LimitMemory(options.LimitMemory);
                cfg.RegexTimeoutInterval(options.RegexTimeoutInterval);
                cfg.TimeoutInterval(options.TimeoutInterval);
                cfg.Modules.RegisterRequire = options.RegisterRequire;
                cfg.StringCompilationAllowed = options.StringCompilationAllowed;
                cfg.Culture = options.Culture;
                cfg.DisableStringCompilation(options.DisableStringCompilation);
                cfg.TimeZone = options.TimeZoneInfo;
                cfg.CancellationToken(default);

                cfg.Interop.SerializeToJson = (obj) => exchange.Encoder(obj);
                cfg.Interop.ExceptionHandler = (ex) => throw new InterpreterInteropException("Interop error", ex);

                if (!string.IsNullOrWhiteSpace(options.ModulesBasePath))
                    cfg.EnableModules(options.ModulesBasePath, true);
            })
            .SetValue("trace", new Action<string, object[]>(logger.LogTrace))
            .SetValue("log", new Action<string, object[]>(logger.LogInformation));


            foreach ((string key, IEngineModule module) in options.ScriptModules)
            {
                Prepared<JsModule> jsModule = moduleParser.GetModule(key, module);
                engine.Modules.Add(key, x =>
                {
                    x.AddModule(jsModule);
                });
            }

            return engine;
        }
    }
    public async ValueTask<RunResult> Run(RunRequest request)
    {
        using (InterpreterTelemetry.Start("run"))
            try
            {
                await semaphore.WaitAsync(request.CancellationToken);
                Exception? exception = null;
                try
                {
                    Prepared<JsScript> script = Prepare(request);
                    JsValue result = instance.Value.Evaluate(script);
                    JsValue raw = result.UnwrapIfPromise();
                }
                catch (Exception innerEx)
                {
                    exception = innerEx;
                    logger.LogWarning(innerEx, "Script run fail");
                }
                return new RunResult
                {
                    Exception = exception,
                };
            }
            catch (Exception ex)
            {
                throw new InterpreterException("Scripting error", ex);
            }
            finally
            {
                semaphore.Release();
            }
    }

    private async ValueTask<(Exception? Exception, JsValue Value)> Exec(RunRequest request)
    {
        using (InterpreterTelemetry.Start("exec"))
            try
            {
                await semaphore.WaitAsync(request.CancellationToken);
                Exception? exception = null;
                JsValue output = JsValue.Undefined;
                try
                {
                    Prepared<JsScript> script = Prepare(request);
                    JsValue result = instance.Value.Evaluate(script);

                    output = result.UnwrapIfPromise();
                }
                catch (Exception innerEx)
                {
                    exception = innerEx;
                    logger.LogWarning(innerEx, "Script evaluation fail");
                }
                return (exception, output);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Script evaluation fail");
                throw new InterpreterException("Scripting error", ex);
            }
            finally
            {
                semaphore.Release();
            }
    }
    public async ValueTask<EvaluationResult<T>> Evaluate<T>(RunRequest request)
         where T : class
    {
        using (InterpreterTelemetry.Start("evaluate_typed"))
        {
            T? output = default;
            (Exception? Exception, JsValue Value) = await Exec(request);
            if (Value.IsUndefined())
                output = default;
            else
            {
                if (Value.IsString())
                    try
                    {
                        output = (T)exchange.Decoder(Value.AsString(), typeof(T))!;
                    }
                    catch { }
                else
                    try
                    {
                        output = Value.TryCast<T>();
                    }
                    catch { }
            }
            return new EvaluationResult<T>
            {
                Exception = Exception,
                Response = output,
            };
        }
    }

    public async ValueTask<EvaluationResult<object>> Evaluate(RunRequest request, Type type)
    {
        using (InterpreterTelemetry.Start("evaluate_object"))
        {
            object? output = null;
            (Exception? Exception, JsValue Value) = await Exec(request);
            if (Value.IsUndefined())
                output = default;
            else
            {
                if (Value.IsString())
                    try
                    {
                        output = exchange.Decoder(Value.AsString(), type)!;
                    }
                    catch { }
                else
                    try
                    {
                        output = Value;
                    }
                    catch { }
            }
            return new EvaluationResult<object>
            {
                Exception = Exception,
                Response = output,
            };
        }
    }

    public async ValueTask<EvaluationResult<bool>> Evaluate(RunRequest request)
    {
        using (InterpreterTelemetry.Start("evaluate_bool"))
        {
            bool output = false;
            (Exception? Exception, JsValue Value) = await Exec(request);
            if (!Value.IsUndefined() && Value.IsBoolean())
            {
                output = Value.AsBoolean();
            }
            return new EvaluationResult<bool>
            {
                Exception = Exception,
                Response = output,
            };
        }
    }

    private Prepared<JsScript> Prepare(RunRequest request)
    {
        using (InterpreterTelemetry.Start("prepare"))
        {
            Engine engine = instance.Value;
            engine.Advanced.ResetCallStack();
            CancellationConstraint? constraint = engine.Constraints.Find<CancellationConstraint>();

            constraint?.Reset(request.CancellationToken);

            if (request.Context is not null)
                engine.SetValue("ctx", exchange.Encoder(request.Context));

            return scriptParser.GetScript(request.Code);
        }
    }

    public void Dispose()
    {
        if (instance.IsValueCreated)
            instance.Value?.Dispose();
        semaphore.Dispose();
    }
}
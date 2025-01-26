namespace Toucan.Sdk.Interpreter;

public interface IInterpreter
{
    ValueTask<RunResult> Run(RunRequest request);
    ValueTask<EvaluationResult<bool>> Evaluate(RunRequest request);
    ValueTask<EvaluationResult<object>> Evaluate(RunRequest request, Type type);
    ValueTask<EvaluationResult<T>> Evaluate<T>(RunRequest request) where T : class;
}

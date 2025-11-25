namespace ToucanHub.Sdk.Contracts.Wrapper;

public static class ResultHelper
{
    public static bool IsNullOrError<TResult>(this TResult? serviceResult)
         where TResult : ResultBase => !serviceResult.IsSuccess();

    public static bool IsNullOrError<TResult>(this TResult? serviceResult, out string[] errors)
         where TResult : ResultBase
    {
        if (serviceResult.IsNullOrError())
        {
            errors = serviceResult.GetMessagesOrDefault();
            return true;
        }
        errors = [];
        return false;
    }

    public static bool IsSuccess<TResult>(this TResult? serviceResult)
        where TResult : ResultBase => serviceResult?.Status != ResultStatus.Error;

    public static string[] GetMessagesOrDefault<TResult>(this TResult? serviceResult, params string[] defaultMessages)
        where TResult : ResultBase => serviceResult?.Messages ?? defaultMessages;
    public static string[] GetMessagesOrDefault<TResult>(this TResult? serviceResult, string? defaultMessage)
        where TResult : ResultBase => serviceResult?.Messages ?? (string.IsNullOrWhiteSpace(defaultMessage) ? [] : [defaultMessage]);
    public static bool IsSuccessWith<T>(this Result<T>? serviceResult, Func<T?, bool> checker, [NotNullWhen(true)] out T? model)
    {
        if ((serviceResult?.IsSuccess() ?? false) && checker(serviceResult.Value))
        {
            model = serviceResult.Value!;
            return true;
        }
        else
        {
            model = default;
            return false;
        }
    }

    public static bool IsSuccessWithModel<T>(this Result<T>? serviceResult, out T model)
        where T : class => serviceResult.IsSuccessWith(x => x != null, out model!);


}

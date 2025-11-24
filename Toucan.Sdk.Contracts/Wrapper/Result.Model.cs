using System.Text.Json.Serialization;

namespace Toucan.Sdk.Contracts.Wrapper;

public record class Result<T> : Result
{
    public new static Result<T> Success(params string[] messages) => Success(default, messages);
    public static Result<T> Success(T? model, params string[] messages) => new()
    {
        Value = model,
        Status = ResultStatus.Success,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };


    public new static Result<T> Error(params string[] messages) => new()
    {
        Status = ResultStatus.Error,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };

    public new static Result<T> Warn(params string[] messages) => Warn(default, messages);
    public static Result<T> Warn(T? value = default, params string[] messages) => new()
    {
        Value = value,
        Status = ResultStatus.Warn,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };

    public static Result<T> Success<TIn>(TIn? model, Converter<TIn, T> converter, params string[] messages) => new()
    {
        Value = model != null ? converter(model) : default,
        Status = ResultStatus.Success,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };
    public static Result<T> Warn<TIn>(TIn? model, Converter<TIn, T> converter, params string[] messages) => new()
    {
        Value = model != null ? converter(model) : default,
        Status = ResultStatus.Warn,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };

    [JsonInclude]
    public T? Value { get; init; }

}

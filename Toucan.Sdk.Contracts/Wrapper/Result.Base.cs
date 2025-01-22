using System.Text.Json.Serialization;

namespace Toucan.Sdk.Contracts.Wrapper;

public enum ResultStatus
{
    Error,
    Warn,
    Success,
}

public record class Result
{

    public static readonly Result EmptySuccess = Success();

    public static Result Success(params string?[] messages) => new()
    {
        Status = ResultStatus.Success,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };
    public static Result Error(params string?[] messages) => new()
    {
        Status = ResultStatus.Error,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };
    public static Result Warn(params string?[] messages) => new()
    {
        Status = ResultStatus.Warn,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };

    [JsonInclude]
    public ResultStatus Status { get; protected init; }

    [JsonInclude]
    public string[] Messages { get; protected init; } = [];

}


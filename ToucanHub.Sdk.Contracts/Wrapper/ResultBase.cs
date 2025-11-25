using System.Text.Json.Serialization;

namespace ToucanHub.Sdk.Contracts.Wrapper;

public record class ResultBase
{

    public static readonly ResultBase EmptySuccess = Success();

    public static ResultBase Success(params string[] messages) => new()
    {
        Status = ResultStatus.Success,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };
    public static ResultBase Error(params string[] messages) => new()
    {
        Status = ResultStatus.Error,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };
    public static ResultBase Warn(params string[] messages) => new()
    {
        Status = ResultStatus.Warn,
        Messages = [.. messages.Where(x => !string.IsNullOrWhiteSpace(x))]
    };

    [JsonInclude]
    public ResultStatus Status { get; protected init; }

    [JsonInclude]
    public string[] Messages { get; protected init; } = [];

}


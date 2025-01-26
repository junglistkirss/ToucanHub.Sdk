namespace Toucan.Sdk.Api.Contracts.Response;

public record class ApiResponseMessage
{
    public required ApiStatus Status { get; init; }
    public string[]? Messages { get; init; }
}

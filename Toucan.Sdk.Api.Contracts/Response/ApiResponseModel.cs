namespace Toucan.Sdk.Api.Contracts.Response;

public record class ApiResponseModel<T> : ApiResponseMessage
{
    public T? Item { get; init; }

}

namespace Toucan.Sdk.Api.Contracts.Response;

public record class ApiResponseModelCollection<T> : ApiResponseModel<ApiCollection<T>>
{
}

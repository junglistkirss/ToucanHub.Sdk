using System.Net.Http.Json;
using Toucan.Sdk.Api.Contracts;
using Toucan.Sdk.Api.Contracts.Response;
using Toucan.Sdk.Api.Contracts.Response.Convention;
using Toucan.Sdk.Contracts;

namespace Toucan.Sdk.Api.Client;


//public interface IToucanApi
//{
//    HttpClient Client { get; }
//    Task GetVersion();
//}

//public class ToucanApi(HttpClient client) : IToucanApi
//{
//    public HttpClient Client { get; } = client;

//    public Task GetVersion() => Task.CompletedTask;
//}


public static partial class ToucanHttpClient
{
    public static async Task<ApiResponseMessage?> GetApiAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            return await client.GetFromJsonAsync<ApiResponseMessage>(requestUri, CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Message(ApiStatus.InternalError, ex.Message);
        }
    }


    public static Task<ApiResponseModel<ModelConvention<TValue>>?> GetApiModelConventionAsync<TValue>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default) => client.GetApiModelAsync<ModelConvention<TValue>>(requestUri, cancellationToken);
    public static async Task<ApiResponseModel<TValue>?> GetApiModelAsync<TValue>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            return await client.GetFromJsonAsync<ApiResponseModel<TValue>>(requestUri, CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Model<TValue>(ApiStatus.InternalError, default!, ex.Message);

        }
    }

    public static Task<ApiResponseModelCollection<ModelConvention<TValue>>?> GetApiConventionCollectionAsync<TValue>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
        => client.GetApiCollectionAsync<ModelConvention<TValue>>(requestUri, cancellationToken);
    public static async Task<ApiResponseModelCollection<TValue>?> GetApiCollectionAsync<TValue>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            return await client.GetFromJsonAsync<ApiResponseModelCollection<TValue>>(requestUri, CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Collection<TValue>(ApiStatus.InternalError, default!, null, ex.Message);
        }
    }
}
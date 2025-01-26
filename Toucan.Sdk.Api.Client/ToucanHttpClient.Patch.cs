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
    public static async Task<ApiResponseMessage?> PatchApiAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PatchAsJsonAsync(requestUri, CommonJson.SerializerOptionsInstance, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseMessage>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Message(ApiStatus.InternalError, ex.Message);

        }
    }

    public static async Task<ApiResponseMessage?> PatchApiAsync<TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PatchAsJsonAsync(requestUri, model, CommonJson.SerializerOptionsInstance, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseMessage>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Message(ApiStatus.InternalError, ex.Message);

        }
    }

    public static Task<ApiResponseModel<ModelConvention<TValue>>?> PatchApiModelConventionAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default) => client.PatchApiModelAsync<ModelConvention<TValue>, TModel>(requestUri, model, cancellationToken);
    public static async Task<ApiResponseModel<TValue>?> PatchApiModelAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PatchAsJsonAsync(requestUri, model, CommonJson.SerializerOptionsInstance, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseModel<TValue>>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Model<TValue>(ApiStatus.InternalError, default!, ex.Message);

        }
    }

    public static Task<ApiResponseModelCollection<ModelConvention<TValue>>?> PatchApiConventionCollectionAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
        => client.PatchApiCollectionAsync<ModelConvention<TValue>, TModel>(requestUri, model, cancellationToken);
    public static async Task<ApiResponseModelCollection<TValue>?> PatchApiCollectionAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PatchAsJsonAsync(requestUri, model, CommonJson.SerializerOptionsInstance, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseModelCollection<TValue>>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Collection<TValue>(ApiStatus.InternalError, default!, null, ex.Message);
        }
    }
}
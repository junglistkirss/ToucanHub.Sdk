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
    public static async Task<ApiResponseModel<TValue>?> PostApiAsync<TValue>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsync(requestUri, null, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseModel<TValue>>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Model<TValue>(ApiStatus.InternalError, default!, ex.Message);

        }
    }


    public static async Task<ApiResponseMessage?> PostApiAsync<TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, model, CommonJson.SerializerOptionsInstance, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseMessage>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Message(ApiStatus.InternalError, ex.Message);

        }
    }

    public static Task<ApiResponseModel<ModelConvention<TValue>>?> PostApiModelConventionAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default) => client.PostApiModelAsync<ModelConvention<TValue>, TModel>(requestUri, model, cancellationToken);

    //public static async Task<ApiResponseModel<TValue>?> PostApiModelAsync<TValue>(this HttpClient client, string requestUri, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        HttpResponseMessage response = await client.PostApiAsync(requestUri, cancellationToken);
    //        return await response.Content.ReadFromJsonAsync<ApiResponseModel<TValue>>(CommonJson.SerializerOptionsInstance, cancellationToken);
    //    }
    //    catch (Exception ex)
    //    {
    //        return ApiHelper.Model<TValue>(default!, ex.Message);

    //    }
    //}

    public static async Task<ApiResponseModel<TValue>?> PostApiModelAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, model, CommonJson.SerializerOptionsInstance, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseModel<TValue>>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Model<TValue>(ApiStatus.InternalError, default!, ex.Message);

        }
    }

    public static Task<ApiResponseModelCollection<ModelConvention<TValue>>?> PostApiConventionCollectionAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
        => client.PostApiCollectionAsync<ModelConvention<TValue>, TModel>(requestUri, model, cancellationToken);
    public static async Task<ApiResponseModelCollection<TValue>?> PostApiCollectionAsync<TValue, TModel>(this HttpClient client, string requestUri, TModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, model, CommonJson.SerializerOptionsInstance, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponseModelCollection<TValue>>(CommonJson.SerializerOptionsInstance, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiHelper.Collection<TValue>(ApiStatus.InternalError, default!, null, ex.Message);
        }
    }
}
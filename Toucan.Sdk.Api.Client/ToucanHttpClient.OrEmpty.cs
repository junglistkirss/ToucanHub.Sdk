using Toucan.Sdk.Api.Contracts;
using Toucan.Sdk.Api.Contracts.Response;

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
    public static async Task<ApiResponseMessage> OrEmpty(this Task<ApiResponseMessage?> apiResponseMessage)
       => (await apiResponseMessage).OrEmpty();
    public static async Task<ApiResponseModel<TValue>> OrEmpty<TValue>(this Task<ApiResponseModel<TValue>?> apiResponseModel)
        => (await apiResponseModel).OrEmpty();

    public static async Task<ApiResponseModelCollection<TValue>> OrEmpty<TValue>(this Task<ApiResponseModelCollection<TValue>?> apiResponseModel)
        => (await apiResponseModel).OrEmpty();

    public static ApiResponseMessage OrEmpty(this ApiResponseMessage? apiResponseModel)
        => apiResponseModel ?? ApiHelper.Message(ApiStatus.NotFound, "Empty");
    public static ApiResponseModel<TValue> OrEmpty<TValue>(this ApiResponseModel<TValue>? apiResponseModel)
        => apiResponseModel ?? ApiHelper.Model<TValue>(ApiStatus.NotFound, default!, "Empty");

    public static ApiResponseModelCollection<TValue> OrEmpty<TValue>(this ApiResponseModelCollection<TValue>? apiResponseModel)
        => apiResponseModel ?? ApiHelper.Collection<TValue>(ApiStatus.NotFound, default!, null, "Empty");
}
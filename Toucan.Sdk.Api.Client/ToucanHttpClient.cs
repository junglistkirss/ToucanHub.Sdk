using System.Net.Http.Json;
using System.Text.RegularExpressions;
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
    [GeneratedRegex(@"{(?<var>\w+)}", RegexOptions.Compiled)]
    private static partial Regex VarPattern();

    static readonly Regex varPattern = VarPattern();

    public static string WithSegments(this string src, Dictionary<string, string> vals)
        => varPattern.Replace(src, m => vals.TryGetValue(m.Groups[1].Value, out string? v) ? v : m.Value);
    public static string WithQueryString(this string basePath, Action<Dictionary<string,object?>> action)
    {
        var args = new Dictionary<string, object?>();
        action(args);
        return basePath.BuildUrlWithQueryStringUsingStringConcat(args.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value!.ToString()!));
    }
    public static string BuildUrlWithQueryStringUsingStringConcat(this string basePath, Dictionary<string, string> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return $"{basePath}?{queryString}";
    }

    public static async Task<ApiResponseMessage?> AsApiMessage(this HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default) => await httpResponseMessage.Content.ReadFromJsonAsync<ApiResponseMessage>(CommonJson.SerializerOptionsInstance, cancellationToken) ;

    public static async Task<ApiResponseModel<T>?> AsApiModel<T>(this HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default) => await httpResponseMessage.Content.ReadFromJsonAsync<ApiResponseModel<T>>(CommonJson.SerializerOptionsInstance, cancellationToken) ;
    public static async Task<ApiResponseModel<ModelConvention<T>>?> AsApiConventionModel<T>(this HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default) => await httpResponseMessage.AsApiModel<ModelConvention<T>>(cancellationToken);

    public static async Task<ApiResponseModelCollection<T>?> AsApiCollection<T>(this HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default) => await httpResponseMessage.Content.ReadFromJsonAsync<ApiResponseModelCollection<T>>(CommonJson.SerializerOptionsInstance,cancellationToken);
    public static async Task<ApiResponseModelCollection<ModelConvention<T>>?> AsApiConventionCollection<T>(this HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default) => await httpResponseMessage.AsApiCollection<ModelConvention<T>>(cancellationToken);
    
}
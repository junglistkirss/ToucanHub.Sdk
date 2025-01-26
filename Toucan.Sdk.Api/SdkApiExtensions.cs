using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Sdk.Api.Features;
using Toucan.Sdk.Api.Middlewares;

namespace Toucan.Sdk.Api;

public static class SdkApiQuery
{
    public const string offset = "offset";
    public const string limit = "limit";
    public const string sortBy = "sortBy";
    public const string sortDirection = "sortDirection";
}

public static class SdkApiHeaders
{
    public const string UseConventionResultHeaderName = "X-Convention-Result";
    public const string ETag = "ETag";
}

//public record struct EnumBinding<T> : IParsable<EnumBinding<T>>
//    where T : struct, Enum
//{
//    private const bool IgnoreCase = true;
//    private const bool IgnoreInt = false;
//    public EnumBinding(){}

//    private T value  = default!;

//    public static bool TryParse(string value, out EnumBinding<T> result)
//    {
//        return TryParse(value, null!, out result);
//    }

//    public static bool TryParse(string? value, IFormatProvider? provider, out EnumBinding<T> result)
//    {
//        result = new EnumBinding<T>();
//        if (IgnoreInt && int.TryParse(value, out _))
//        {
//            return false;
//        }

//        var success = Enum.TryParse(value, IgnoreCase, out T parsedValue);
//        if (!success || !Enum.IsDefined(typeof(T), parsedValue))
//        {
//            return false;
//        }

//        result.value = parsedValue;
//        return true;
//    }

//    public static implicit operator T(EnumBinding<T> e) => e.value;

//    public override string ToString() => value.ToString();

//    public static EnumBinding<T> Parse(string s, IFormatProvider? provider)
//    {
//        if (TryParse(s, provider, out EnumBinding<T> result))
//            return result;
//        throw new InvalidCastException();
//    }
//}

public static class SdkApiExtensions
{
    public static IServiceCollection UseApiResults(this IServiceCollection services)
    {
        return services.AddScoped(s =>
        {
            HttpContext? httpContext = s.GetRequiredService<IHttpContextAccessor>().HttpContext;
            return new ApiResults(httpContext?.Request.Headers.ContainsKey(SdkApiHeaders.UseConventionResultHeaderName) ?? false, s);
        });
    }

    public static IApplicationBuilder UseToucanSecurityMiddlewares(this IApplicationBuilder services)
    {
        return services
            .UseMiddleware<ApiUserContextResolver>()
            .UseMiddleware<ContextResolver>();
    }
}

using System.Diagnostics.CodeAnalysis;
using Toucan.Sdk.Api.Contracts.Response;

namespace Toucan.Sdk.Api.Contracts;

public static class ApiHelper
{
    public static bool IsSuccessNotEmpty<T>(this ApiResponseModelCollection<T>? message, [NotNullWhen(true)] out T[]? model, out long? domainCount)
    {
        if (message.IsSuccessWithCheck(x => x is not null && x.Count > 0, out ApiCollection<T>? collection))
        {
            model = collection!.Collection;
            domainCount = collection!.DomainCount;
            return true;
        }
        model = default;
        domainCount = message!.Item!.DomainCount;
        return false;
    }
    public static bool IsSuccessWithModel<T>(this ApiResponseModel<T>? message, [NotNullWhen(true)] out T? model)
    {
        if (message.IsSuccessWithCheck(x => x is not null, out model))
        {
            model = message!.Item!;
            return true;
        }
        model = default;
        return false;
    }
    public static bool IsSuccessWithCheck<T>(this ApiResponseModel<T>? message, Func<T?, bool> check, [MaybeNullWhen(true)] out T? model)
    {
        bool success = message.IsSuccess() && check(message!.Item);
        if (success)
        {
            model = message!.Item;
            return true;
        }
        else
        {
            model = default;
            return false;
        }
    }

    public static bool IsSuccess(this ApiResponseMessage? message) => message?.Status is ApiStatus.Success;
    public static bool IsSuccess(this ApiStatus status) => status is ApiStatus.Success;
    public static bool IsError(this ApiResponseMessage? message, out string[] messages)
    {
        if (!message.IsSuccess())
        {
            messages = message?.Messages ?? [];
            return true;
        }
        messages = [];
        return false;
    }
    public static ApiResponseMessage Message(ApiStatus status, params string[] messages) => new()
    {
        Status = status,
        Messages = messages,
    };

    public static ApiResponseModel<TOut> Model<TIn, TOut>(ApiStatus status, TIn model, Converter<TIn, TOut> converter, params string[] message) => new()
    {
        Status = status,
        Item = converter(model),
        Messages = message,
    };

    //public static ApiResponseModelCollection<T> Collection<TIn, T>(PartialCollection<TIn>? models, Converter<TIn, T> converter, string? message = null)
    //{
    //    return new ApiResponseModelCollection<T>(new ApiCollection<T>
    //    {
    //        Collection = models?.ToList().ConvertAll(converter),
    //        DomainCount = models?.DomainCount,
    //    }, message);
    //}

    public static ApiResponseModelCollection<T> Collection<TIn, T>(ApiStatus status, IEnumerable<TIn> models, Converter<TIn, T> converter, long? domainCount = null, params string[] messages)
    {
        return new ApiResponseModelCollection<T>()
        {
            Status = status,
            Item = new ApiCollection<T>
            {
                Collection = models != null ? [.. models.Select(x => converter(x))] : [],
                DomainCount = domainCount,
            },
            Messages = messages,
        };
    }

    public static ApiResponseModel<T> Model<T>(ApiStatus status, T? model, params string[] messages) => new()
    {
        Status = status,
        Item = model,
        Messages = messages,
    };

    public static ApiResponseModelCollection<T> Collection<T>(ApiStatus status, IEnumerable<T> models, long? domainCount = null, params string[] messages)
    {
        return new ApiResponseModelCollection<T>()
        {
            Status = status,
            Item = new ApiCollection<T>
            {
                Collection = models != null ? [.. models] : [],
                DomainCount = domainCount,
            },
            Messages = messages,
        };
    }
}

using System.Net.Mime;
using Toucan.Sdk.Api.Contracts;
using Toucan.Sdk.Contracts;
using IApiResult = Microsoft.AspNetCore.Http.IResult;
using IMessageResult = Toucan.Sdk.Contracts.Wrapper.Result;
using ITypedApiResult = Microsoft.AspNetCore.Http.TypedResults;

namespace Toucan.Sdk.Api.Extensions;
public static class ApiResultsHelper
{
    private const string json_contenttype = MediaTypeNames.Application.Json;

    public static IApiResult Raw<T>(T data)
       => Results.Json(
           data,
           options: CommonJson.SerializerOptionsInstance,
           contentType: json_contenttype,
           statusCode: StatusCodes.Status200OK);

    internal static IApiResult UnauthorizedMessage(params string[] messages)
        => Results.Json(
            ApiHelper.Message(ApiStatus.Unauthorized, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status401Unauthorized);

    internal static IApiResult ForbiddenMessage(params string[] messages)
        => Results.Json(
            ApiHelper.Message(ApiStatus.Forbidden, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status403Forbidden);

    internal static IApiResult NoResultMessage(params string[] messages)
        => Results.Json(
            ApiHelper.Message(ApiStatus.NotFound, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status204NoContent);

    internal static IApiResult NotFoundMessage(params string[] messages)
        => Results.Json(
            ApiHelper.Message(ApiStatus.NotFound, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status404NotFound);

    internal static IApiResult BadRequestMessage(params string[] messages)
        => Results.Json(
            ApiHelper.Message(ApiStatus.Failure, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status400BadRequest);

    internal static IApiResult InternalErrorMessage(params string[] messages)
        => Results.Json(
            ApiHelper.Message(ApiStatus.InternalError, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status500InternalServerError);

    internal static IApiResult OkMessage(params string[] messages)
        => Results.Json(
            ApiHelper.Message(ApiStatus.Success, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status200OK);

    internal static IApiResult FromMessageResult(IMessageResult? result, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (result is null)
            return NoResultMessage(message?.Invoke(null) ?? result.GetMessagesOrDefault(defaultErrorMessage));

        if (result.IsSuccess())
            return OkMessage(message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultSuccessMessage));
        return BadRequestMessage(message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultErrorMessage));
    }

    internal static IApiResult FromResult<T>(Result<T>? result, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (result is null)
            return NoResultMessage(message?.Invoke(null) ?? result.GetMessagesOrDefault(defaultErrorMessage));

        if (result.IsSuccess())
            return OkModel(result.Value, message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultSuccessMessage));
        return ErrorModel(result.Value, message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultErrorMessage));
    }

    internal static IApiResult FromResults<T>(Results<T>? result, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (result is null)
            return NoResultMessage(message?.Invoke(null) ?? result.GetMessagesOrDefault(defaultErrorMessage));

        if (result.IsSuccess())
            return OkCollection(result.Values ?? [], result.DomainCount, message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultSuccessMessage));
        return ErrorCollection(result.Values ?? [], message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultErrorMessage));
    }
    internal static IApiResult OkModel<T>(T? model, params string[] messages)
        => ITypedApiResult.Json(
            ApiHelper.Model(ApiStatus.Success, model, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status200OK);

    internal static IApiResult ErrorModel<T>(T? model, params string[] messages)
        => Results.Json(
            ApiHelper.Model(ApiStatus.Failure, model, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status400BadRequest);

    internal static IApiResult MapFromResult<TIn, TOut>(Result<TIn>? result, Converter<TIn?, TOut> converter, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (result is null)
            return NoResultMessage(message?.Invoke(null) ?? result.GetMessagesOrDefault(defaultErrorMessage));

        if (result.IsSuccess())
            return OkMapModel(result.Value, converter, message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultSuccessMessage));
        return ErrorMapModel(result.Value, converter, message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultErrorMessage));
    }
    internal static IApiResult OkMapModel<TIn, TOut>(TIn model, Converter<TIn, TOut> converter, params string[] messages)
        => OkModel(converter(model), messages);

    internal static IApiResult ErrorMapModel<TIn, TOut>(TIn? model, Converter<TIn, TOut> converter, params string[] messages)
        => ErrorModel(model is not null ? converter(model) : default, messages);


    //internal static IApiResult FromResults<T>(Results<T>? result, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    //{
    //    if (result is null)
    //        return NoResultMessage(message?.Invoke(null) ?? result.GetMessagesOrDefault(defaultErrorMessage));

    //    if (result.IsSuccess())
    //        return OkCollection(result.Values ?? [], message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultSuccessMessage));
    //    return ErrorCollection(result.Values ?? [], message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultErrorMessage));
    //}

    internal static IApiResult OkCollection<T>(IEnumerable<T> models, long? domainCout, params string[] messages)
        => Results.Json(
            ApiHelper.Collection(ApiStatus.Success, models, domainCout, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status200OK);

    internal static IApiResult OkMapCollection<TIn, TOut>(IEnumerable<TIn> models, Converter<TIn, TOut> converter, long? domainCount = null, params string[] messages)
    {
        var transform = ApiHelper.Collection(ApiStatus.Success, models, converter, domainCount, messages);
        return Results.Json(
                transform,
                options: CommonJson.SerializerOptionsInstance,
                contentType: json_contenttype,
                statusCode: StatusCodes.Status200OK);
    }

    internal static IApiResult MapFromResults<TIn, TOut>(Results<TIn>? result, Converter<TIn, TOut> converter, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (result is null)
            return NoResultMessage(message?.Invoke(null) ?? result.GetMessagesOrDefault(defaultErrorMessage));

        if (result.IsSuccess())
            return OkMapCollection(result.Values ?? [], converter, result.DomainCount, message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultSuccessMessage));
        return ErrorMapCollection(result.Values ?? [], converter, message?.Invoke(result.Status) ?? result.GetMessagesOrDefault(defaultErrorMessage));
    }

    internal static IApiResult ErrorMapCollection<TIn, T>(IEnumerable<TIn> models, Converter<TIn, T> converter, params string[] messages)
        => Results.Json(
            ApiHelper.Collection(ApiStatus.Failure, models, converter, null, messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status400BadRequest);

    internal static IApiResult ErrorCollection<T>(IEnumerable<T> models, params string[] messages)
        => Results.Json(
            ApiHelper.Collection(ApiStatus.Failure, models, messages: messages),
            options: CommonJson.SerializerOptionsInstance,
            contentType: json_contenttype,
            statusCode: StatusCodes.Status400BadRequest);
}

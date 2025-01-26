using Microsoft.Extensions.DependencyInjection;
using Toucan.Sdk.Api.Contracts.Response.Convention;
using Toucan.Sdk.Api.Extensions;
using IApiResult = Microsoft.AspNetCore.Http.IResult;
using IMessageResult = Toucan.Sdk.Contracts.Wrapper.Result;

namespace Toucan.Sdk.Api.Features;

#pragma warning disable CA1822 // Marquer les membres comme étant static
public class ApiResults(bool useConvention, IServiceProvider serviceProvider)
{
    private ConventionMapper<T> GetConvention<T>()
    {
        return serviceProvider.GetService<ConventionMapper<T>>() ?? ModelConventionMapper<T>.Self();
    }

    public IApiResult UnauthorizedMessage(params string[] messages) => ApiResultsHelper.UnauthorizedMessage(messages);
    public IApiResult ForbiddenMessage(params string[] messages) => ApiResultsHelper.ForbiddenMessage(messages);
    public IApiResult NoResultMessage(params string[] messages) => ApiResultsHelper.NoResultMessage(messages);
    public IApiResult NotFoundMessage(params string[] messages) => ApiResultsHelper.NotFoundMessage(messages);
    public IApiResult BadRequestMessage(params string[] messages) => ApiResultsHelper.BadRequestMessage(messages);
    public IApiResult InternalErrorMessage(params string[] messages) => ApiResultsHelper.InternalErrorMessage(messages);
    public IApiResult OkMessage(params string[] messages) => ApiResultsHelper.OkMessage(messages);
    public IApiResult FromMessageResult(IMessageResult? result, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
        => ApiResultsHelper.FromMessageResult(result, message, defaultSuccessMessage, defaultErrorMessage);

    public IApiResult FromResult<T>(Result<T>? result, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (useConvention)
        {
            ConventionMapper<T> convention = GetConvention<T>();
            return ApiResultsHelper.MapFromResult(result, x => x is null ? null : convention(x), message, defaultSuccessMessage, defaultErrorMessage);
        }
        return ApiResultsHelper.FromResult(result, message, defaultSuccessMessage, defaultErrorMessage);
    }

    public IApiResult FromResults<T>(Results<T>? result, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (useConvention)
        {
            ConventionMapper<T> convention = GetConvention<T>();
            return ApiResultsHelper.MapFromResults(result, x => convention(x), message, defaultSuccessMessage, defaultErrorMessage);
        }
        return ApiResultsHelper.FromResults<T>(result, message, defaultSuccessMessage, defaultErrorMessage);
    }
    public IApiResult OkModel<T>(T? model, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<T> convention = GetConvention<T>();
            return ApiResultsHelper.OkMapModel(model, x => x is null ? null : convention(x), messages);
        }
        return ApiResultsHelper.OkModel(model, messages);
    }
    public IApiResult ErrorModel<T>(T? model, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<T> convention = GetConvention<T>();
            return ApiResultsHelper.ErrorMapModel(model, x => convention(x), messages);
        }
        return ApiResultsHelper.ErrorModel(model, messages);
    }

    public IApiResult MapFromResult<TIn, TOut>(Result<TIn>? result, Converter<TIn?, TOut> converter, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (useConvention)
        {
            ConventionMapper<TOut> convention = GetConvention<TOut>();
            return ApiResultsHelper.MapFromResult(result, (x) => convention(converter(x)), message, defaultSuccessMessage, defaultErrorMessage);
        }
        return ApiResultsHelper.MapFromResult(result, converter, message, defaultSuccessMessage, defaultErrorMessage);
    }
    public IApiResult OkMapModel<TIn, TOut>(TIn model, Converter<TIn, TOut> converter, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<TOut> convention = GetConvention<TOut>();
            return ApiResultsHelper.OkMapModel(model, (x) => convention(converter(x)), messages);

        }
        return ApiResultsHelper.OkMapModel(model, converter, messages);
    }
    public IApiResult ErrorMapModel<TIn, TOut>(TIn? model, Converter<TIn, TOut> converter, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<TOut> convention = GetConvention<TOut>();
            return ApiResultsHelper.ErrorMapModel(model, (x) => convention(converter(x)), messages);
        }
        return ApiResultsHelper.ErrorMapModel(model, converter, messages);
    }
    public IApiResult OkCollection<T>(IEnumerable<T> models, long? domainCount, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<T> convention = GetConvention<T>();
            return ApiResultsHelper.OkMapCollection(models, x => convention(x), null, messages);
        }
        return ApiResultsHelper.OkCollection(models, domainCount, messages);
    }
    public IApiResult OkMapCollection<TIn, TOut>(IEnumerable<TIn> models, Converter<TIn, TOut> converter, long? domainCount = null, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<TOut> convention = GetConvention<TOut>();
            return ApiResultsHelper.OkMapCollection(models, (x) => convention(converter(x)), domainCount, messages);
        }
        return ApiResultsHelper.OkMapCollection(models, converter, domainCount, messages);
    }
    public IApiResult MapFromResults<TIn, TOut>(Results<TIn>? result, Converter<TIn, TOut> converter, Func<ResultStatus?, string[]>? message = null, string? defaultSuccessMessage = null, string? defaultErrorMessage = null)
    {
        if (useConvention)
        {
            ConventionMapper<TOut> convention = GetConvention<TOut>();
            return ApiResultsHelper.MapFromResults(result, (x) => convention(converter(x)), message, defaultSuccessMessage, defaultErrorMessage);
        }
        return ApiResultsHelper.MapFromResults(result, converter, message, defaultSuccessMessage, defaultErrorMessage);
    }
    public IApiResult ErrorMapCollection<TIn, T>(IEnumerable<TIn> models, Converter<TIn, T> converter, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<T> convention = GetConvention<T>();
            return ApiResultsHelper.ErrorMapCollection(models, (x) => convention(converter(x)), messages);
        }
        return ApiResultsHelper.ErrorMapCollection(models, converter, messages);
    }
    public IApiResult ErrorCollection<T>(IEnumerable<T> models, params string[] messages)
    {
        if (useConvention)
        {
            ConventionMapper<T> convention = GetConvention<T>();
            return ApiResultsHelper.ErrorMapCollection(models, x => convention(x), messages);
        }
        return ApiResultsHelper.ErrorCollection(models, messages);
    }
}
#pragma warning restore CA1822 // Marquer les membres comme étant static

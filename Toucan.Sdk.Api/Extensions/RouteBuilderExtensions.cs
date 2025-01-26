using Microsoft.AspNetCore.Builder;
using Toucan.Sdk.Api.Contracts.Response.Convention;

namespace Toucan.Sdk.Api.Extensions;

public static class RouteBuilderExtensions
{
    public static RouteHandlerBuilder ProducesMessage(this RouteHandlerBuilder builder)
    {
        return builder
            .ProducesOrFail<ApiResponseMessage>();
    }
    public static RouteHandlerBuilder ProducesModel<TModel>(this RouteHandlerBuilder builder)
    {
        return builder
            .ProducesOrFail<ApiResponseModel<TModel>>()
            .ProducesOrFail<ApiResponseModel<ModelConvention<TModel>>>();
    }
    public static RouteHandlerBuilder ProducesCollection<TModel>(this RouteHandlerBuilder builder)
    {
        return builder
            .ProducesOrFail<ApiResponseModelCollection<TModel>>()
            .ProducesOrFail<ApiResponseModelCollection<ModelConvention<TModel>>>();
    }
    public static RouteHandlerBuilder ProducesOrFail<TResponse>(this RouteHandlerBuilder builder)
    {
        return builder.Produces<TResponse>().MayProducesErrors();
    }

    public static RouteHandlerBuilder MayProducesErrors(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<ApiResponseMessage>(StatusCodes.Status500InternalServerError)
            .Produces<ApiResponseMessage>(StatusCodes.Status401Unauthorized)
            .Produces<ApiResponseMessage>(StatusCodes.Status403Forbidden)
            .Produces<ApiResponseMessage>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseMessage>(StatusCodes.Status400BadRequest);
    }

}

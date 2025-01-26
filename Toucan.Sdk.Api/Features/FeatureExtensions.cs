using Microsoft.Extensions.DependencyInjection;
using Toucan.Sdk.Application.Context;

namespace Toucan.Sdk.Api.Features;

public static class FeatureExtensions
{
    internal static IServiceCollection UseFeatureApiContextResolver(this IServiceCollection services) =>
        services.AddScoped(x => x.GetRequiredService<IHttpContextAccessor>().HttpContext?.Features.Get<IContext>() ?? ApiContext.Empty);
}

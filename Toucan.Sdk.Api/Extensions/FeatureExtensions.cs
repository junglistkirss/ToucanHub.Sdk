using Microsoft.Extensions.DependencyInjection;
using Toucan.Identity.Api.Features;
using Toucan.Sdk.Application.Context;

namespace Toucan.Sdk.Api.Extensions;
public static class FeatureExtensions
{
    public static IServiceCollection UseApiFeatureContext(this IServiceCollection services) =>
        services.AddScoped(x => x.GetRequiredService<IHttpContextAccessor>().HttpContext?.Features.Get<IUserContext>() ?? ApiUserContext.CreateEmpty());
}

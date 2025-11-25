using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToucanHub.Sdk.Pipeline.Internal;

namespace ToucanHub.Sdk.Pipeline;

public static class RegisterPipelines
{
    public static IServiceCollection UsePipelines(this IServiceCollection serviceDescriptors)
    {
        serviceDescriptors.TryAddTransient(typeof(IAsyncPipeline<>), typeof(AsyncPipeline<>));
        serviceDescriptors.TryAddTransient(typeof(IPipeline<>), typeof(SimplePipeline<>));

        return serviceDescriptors;
    }
}
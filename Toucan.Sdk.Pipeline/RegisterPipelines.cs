using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toucan.Sdk.Pipeline.Internal;

namespace Toucan.Sdk.Pipeline;

public static class RegisterPipelines
{
    public static IServiceCollection UsePipelines(this IServiceCollection serviceDescriptors)
    {
        serviceDescriptors.TryAddTransient(typeof(IAsyncPipeline<>), typeof(AsyncPipeline<>));
        serviceDescriptors.TryAddTransient(typeof(IPipeline<>), typeof(SimplePipeline<>));

        return serviceDescriptors;
    }
}
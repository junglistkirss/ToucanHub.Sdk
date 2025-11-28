using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToucanHub.Sdk.Pipeline.Internal;

namespace ToucanHub.Sdk.Pipeline;

public static class RegisterPipelines
{
    public static IServiceCollection UsePipelines(this IServiceCollection serviceDescriptors, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        return serviceDescriptors
            .UseAsyncPipelines(lifetime)
            .UseSyncPipelines(lifetime);
    }

    public static IServiceCollection UseAsyncPipelines(this IServiceCollection serviceDescriptors, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        serviceDescriptors.TryAdd(new ServiceDescriptor(typeof(IAsyncPipeline<>), typeof(AsyncPipeline<>), lifetime));
        return serviceDescriptors;
    }

    public static IServiceCollection UseSyncPipelines(this IServiceCollection serviceDescriptors, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        serviceDescriptors.TryAdd(new ServiceDescriptor(typeof(IPipeline<>), typeof(Pipeline<>), lifetime));
        return serviceDescriptors;
    }
}
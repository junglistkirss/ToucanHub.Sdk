using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Sdk.Reactive;
public static class ReactiveModule
{
    public static IServiceCollection AddReactiveHostedService(this IServiceCollection services)
    {
        services.AddSingleton<ReactiveSubscriptions>();
        services.AddSingleton<ISubscriptionManager>(s => s.GetRequiredService<ReactiveSubscriptions>());
        services.AddSingleton<ISubscriptionDispatcher>(s => s.GetRequiredService<ReactiveSubscriptions>());
        services.AddHostedService(sp => sp.GetRequiredService<ReactiveSubscriptions>());
        return services;
    }

    public static IServiceCollection AddManagedReactiveHostedService<TServiceId>(this IServiceCollection services, GenerateServiceId<TServiceId> generateServiceId)
        where TServiceId : IEquatable<TServiceId>
    {
        services.AddSingleton(generateServiceId);
        services.AddTransient<ManagedReactive>();
        services.AddSingleton<SharedReactive<TServiceId>>();
        services.AddSingleton<IReactiveLauncher<TServiceId>>(s => s.GetRequiredService<SharedReactive<TServiceId>>());
        services.AddSingleton<IReactiveManagedDispatcher<TServiceId>>(s => s.GetRequiredService<SharedReactive<TServiceId>>());
        services.AddSingleton<IReactiveManagedSubscriber<TServiceId>>(s => s.GetRequiredService<SharedReactive<TServiceId>>());
        services.AddHostedService(sp => sp.GetRequiredService<SharedReactive<TServiceId>>());

        return services;
    }
}


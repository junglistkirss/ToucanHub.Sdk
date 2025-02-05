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


        services.AddSingleton<SharedReactive>();
        services.AddTransient<ManagedReactive>();
        services.AddSingleton<IReactiveLauncher>(s => s.GetRequiredService<SharedReactive>());
        services.AddSingleton<IReactiveManagedDispatcher>(s => s.GetRequiredService<SharedReactive>());
        services.AddSingleton<IReactiveManagedSubscriber>(s => s.GetRequiredService<SharedReactive>());
        services.AddHostedService(sp => sp.GetRequiredService<SharedReactive>());

        return services;
    }
}


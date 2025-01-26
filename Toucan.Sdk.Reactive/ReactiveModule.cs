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
}


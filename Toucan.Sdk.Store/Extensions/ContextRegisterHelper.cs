using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Sdk.Store.Services;
using Toucan.Sdk.Store.Services.Internals;

namespace Toucan.Sdk.Store.Extensions;

public static class ContextRegisterHelper
{
    public static IServiceCollection AddPersistence<TProxy, TContext>(this IServiceCollection services)
        where TProxy : IWriteContextProxy
        where TContext : DbContext, TProxy
    {
        return services.AddScoped<IContextModelPersistence<TProxy>, DbContextModelPersistence<TProxy, TContext>>();
    }

    public static IServiceCollection AddQueryHelper<TProxy, TContext>(this IServiceCollection services)
        where TProxy : IReadContextProxy
        where TContext : DbContext, TProxy
    {
        static DbContextQueryHelper<TProxy, TContext> SameInstance(IServiceProvider provider) => provider.GetRequiredService<DbContextQueryHelper<TProxy, TContext>>();

        return services
            .AddScoped<DbContextQueryHelper<TProxy, TContext>>()
            .AddScoped<IContextQueryHelper<TProxy>>(SameInstance)
            .AddScoped<IContextQueryModelHelper<TProxy>>(SameInstance)
            .AddScoped<IContextQueryCollectionHelper<TProxy>>(SameInstance);
    }
}

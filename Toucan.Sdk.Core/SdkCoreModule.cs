using Microsoft.Extensions.DependencyInjection;
using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Core;

public static class SdkCoreModule
{
    public static IServiceCollection RegisterQueryHandler<TQuery, THandler, TResponse>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TQuery : class, IQuery
        where THandler : class, IQueryHandler<TQuery, TResponse>
        where TResponse : class => lifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient<IQueryHandler<TQuery, TResponse>, THandler>(),
            ServiceLifetime.Scoped => services.AddScoped<IQueryHandler<TQuery, TResponse>, THandler>(),
            ServiceLifetime.Singleton => services.AddSingleton<IQueryHandler<TQuery, TResponse>, THandler>(),
            _ => throw new InvalidOperationException(),
        };

    public static IServiceCollection RegisterCommandHandler<TCommand, THandler>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TCommand : class, ICommand
        where THandler : class, ICommandHandler<TCommand>
        => lifetime switch
        {
            ServiceLifetime.Scoped => services.AddScoped<ICommandHandler<TCommand>, THandler>(),
            ServiceLifetime.Transient => services.AddTransient<ICommandHandler<TCommand>, THandler>(),
            ServiceLifetime.Singleton => services.AddSingleton<ICommandHandler<TCommand>, THandler>(),
            _ => throw new InvalidOperationException(),
        };

    public static IServiceCollection RegisterCommandHandler<TCommand, THandler, TResponse>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TCommand : class, ICommand
        where THandler : class, ICommandHandler<TCommand, TResponse>
        where TResponse : class
        => lifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient<ICommandHandler<TCommand, TResponse>, THandler>(),
            ServiceLifetime.Scoped => services.AddScoped<ICommandHandler<TCommand, TResponse>, THandler>(),
            ServiceLifetime.Singleton => services.AddSingleton<ICommandHandler<TCommand, TResponse>, THandler>(),
            _ => throw new InvalidOperationException(),
        };

    public static IServiceCollection RegisterEventHandler<TEvent, THandler>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TEvent : class, IEvent
        where THandler : class, IEventHandler<TEvent>
            => lifetime switch
            {
                ServiceLifetime.Transient => services.AddTransient<IEventHandler<TEvent>, THandler>(),
                ServiceLifetime.Scoped => services.AddScoped<IEventHandler<TEvent>, THandler>(),
                ServiceLifetime.Singleton => services.AddSingleton<IEventHandler<TEvent>, THandler>(),
                _ => throw new InvalidOperationException(),
            };
}


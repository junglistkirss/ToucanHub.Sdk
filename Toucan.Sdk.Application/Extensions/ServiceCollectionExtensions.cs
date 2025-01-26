using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Sdk.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RedirectScoped<TInterface, TRedirect>(this IServiceCollection services)
        where TInterface : class
        where TRedirect : class, TInterface
    {
        services.AddScoped<TInterface>(provider => provider.GetRequiredService<TRedirect>());
        return services;
    }

    public static IServiceCollection RedirectTransient<TInterface, TRedirect>(this IServiceCollection services)
        where TInterface : class
        where TRedirect : class, TInterface
    {
        services.AddTransient<TInterface>(provider => provider.GetRequiredService<TRedirect>());
        return services;
    }

    public static IServiceCollection RedirectSingleton<TInterface, TRedirect>(this IServiceCollection services)
        where TInterface : class
        where TRedirect : class, TInterface
    {
        services.AddSingleton<TInterface>(provider => provider.GetRequiredService<TRedirect>());
        return services;
    }
}

//using Microsoft.Extensions.DependencyInjection;
//using System.Reflection;

//namespace Toucan.Sdk.Application.Extensions;
//public static class ServiceCollectionExtensions
//{
//    public delegate Type GenericTypeResolver(Type baseType, Type interfaceType);

//    public static void RegisterAll<T>(this IServiceCollection services, Assembly[] assemblies,
//        ServiceLifetime lifetime = ServiceLifetime.Transient) => services.RegisterAll(typeof(T), assemblies, null, lifetime);

//    public static void AddScopedFromAssembly<T>(
//        this IServiceCollection services,
//        params Assembly[] assemblies
//    ) => services.AddScopedFromAssembly(typeof(T), assemblies);

//    public static void AddSingletonFromAssembly<T>(
//        this IServiceCollection services,
//        params Assembly[] assemblies
//    ) => services.AddSingletonFromAssembly(typeof(T), assemblies);

//    public static void AddTransientFromAssembly<T>(
//        this IServiceCollection services,
//        params Assembly[] assemblies
//    ) => services.AddTransientFromAssembly(typeof(T), assemblies);

//    public static void AddScopedFromAssembly(
//        this IServiceCollection services,
//        Type baseType,
//        params Assembly[] assemblies
//    ) => services.RegisterAll(baseType, assemblies, null, ServiceLifetime.Scoped);

//    public static void AddScopedFromAssembly(
//        this IServiceCollection services,
//        Type baseType,
//        GenericTypeResolver typeResolver,
//        params Assembly[] assemblies
//    ) => services.RegisterAll(baseType, assemblies, typeResolver, ServiceLifetime.Scoped);

//    public static void AddTransientFromAssembly(
//        this IServiceCollection services,
//        Type baseType,
//        params Assembly[] assemblies
//    ) => services.RegisterAll(baseType, assemblies, null, ServiceLifetime.Transient);

//    public static void AddSingletonFromAssembly(
//        this IServiceCollection services,
//        Type baseType,
//        params Assembly[] assemblies
//    ) => services.RegisterAll(baseType, assemblies, null, ServiceLifetime.Singleton);

//    public static void RegisterAll(
//        this IServiceCollection services,
//        Type baseType,
//        Assembly[] assemblies,
//        GenericTypeResolver? genericTypeResolver,
//        ServiceLifetime lifetime = ServiceLifetime.Transient)
//    {
//        foreach (Assembly assembly in assemblies)
//        {
//            foreach (Type type in assembly.DefinedTypes)
//            {
//                if (!type.IsAbstract)
//                {
//                    if (baseType.IsInterface)
//                    {
//                        foreach (Type i in type.GetInterfaces())
//                        {
//                            if (baseType.IsGenericTypeDefinition && i.IsGenericType)
//                            {
//                                Type? baseTypeGenericTypeDefinition = baseType.GetGenericTypeDefinition();
//                                if (baseTypeGenericTypeDefinition == i.GetGenericTypeDefinition())
//                                {
//                                    if (type.IsGenericType)
//                                    {
//                                        if (genericTypeResolver != null)
//                                        {
//                                            //services.Add(new ServiceDescriptor(type, lifetime));
//                                            services.Add(new ServiceDescriptor(
//                                                baseTypeGenericTypeDefinition.MakeGenericType(i.GetGenericArguments()),
//                                                s =>
//                                                {
//                                                    return s.GetRequiredService(genericTypeResolver(baseType, type));
//                                                }, ServiceLifetime.Singleton));
//                                        }
//                                    }
//                                    else
//                                    {
//                                        services.Add(new ServiceDescriptor(baseTypeGenericTypeDefinition.MakeGenericType(i.GetGenericArguments()), type, lifetime));
//                                    }
//                                }
//                            }
//                            else if (i == baseType)
//                            {
//                                //if (type.IsGenericType)
//                                //{ }
//                                //else
//                                services.Add(new ServiceDescriptor(baseType, type, lifetime));
//                            }
//                        }
//                    }
//                    else if (baseType.IsGenericTypeDefinition && type.IsGenericType)
//                    {
//                        Type? baseTypeGenericTypeDefinition = baseType.GetGenericTypeDefinition();
//                        if (baseTypeGenericTypeDefinition == type.GetGenericTypeDefinition())
//                            services.Add(new ServiceDescriptor(baseTypeGenericTypeDefinition.MakeGenericType(type.GetGenericArguments()), type, lifetime));
//                    }
//                    else if (type.IsAssignableTo(baseType))
//                    {
//                        services.Add(new ServiceDescriptor(baseType, type, lifetime));
//                    }
//                }
//            }
//        }
//    }
//}

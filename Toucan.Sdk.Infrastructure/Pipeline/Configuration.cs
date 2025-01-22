using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Infrastructure.Pipeline;

public static class Configuration
{
    public static IServiceCollection AddMessagePipeline(this IServiceCollection services) =>
        services.AddSingleton<IMessagePipeline, MessagePipeline>();

    public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services, T eventHandler)
        where T : class, IMessageMiddleware =>
        services.AddScoped<IMessageMiddleware>(_ => eventHandler);

    public static IServiceCollection AddMessageHandler<T>(this IServiceCollection services)
        where T : class, IMessageMiddleware =>
        services.AddScoped<IMessageMiddleware, T>();

    public static IServiceCollection Handle<TMessage>(
        this IServiceCollection services,
        Action<TMessage> handler) =>
        services.AddMessageHandler(new MessageHandlerWrapper<TMessage>(handler));

    public static IServiceCollection Handle<TMessage>(
        this IServiceCollection services,
        Func<TMessage, CancellationToken, ValueTask> handler) =>
        services.AddMessageHandler(new MessageHandlerWrapper<TMessage>(handler));

    public static IServiceCollection Handle<TMessage>(
        this IServiceCollection services,
        IMessageMiddleware<TMessage> handler) =>
        services.AddMessageHandler(handler);

    public static IServiceCollection Transform<TMessage, TTransformedMessage>(
        this IServiceCollection services,
        Func<TMessage, TTransformedMessage> handler) =>
        services.AddMessageHandler(new MessageTransformationWrapper<TMessage, TTransformedMessage>(handler));


    public static IServiceCollection Transform<TMessage>(
        this IServiceCollection services,
        Func<TMessage, object> handler) =>
        services.AddMessageHandler(new MessageTransformationWrapper<TMessage>(handler));

    public static IServiceCollection Transform<TMessage, TTransformedMessage>(
        this IServiceCollection services,
        Func<TMessage, CancellationToken, ValueTask<TTransformedMessage>> handler) =>
        services.AddMessageHandler(new MessageTransformationWrapper<TMessage, TTransformedMessage>(handler));


    public static IServiceCollection Transform<TMessage>(
        this IServiceCollection services,
        Func<TMessage, CancellationToken, ValueTask<object>> handler) =>
        services.AddMessageHandler(new MessageTransformationWrapper<TMessage>(handler));

    public static IServiceCollection Transform<TMessage, TTransformedMessage>(
        this IServiceCollection services,
        IMessageTransformation<TMessage, TTransformedMessage> handler) =>
        services.AddMessageHandler(handler);

    public static IServiceCollection Filter<TMessage>(
        this IServiceCollection services,
        Func<TMessage, bool> handler) =>
        services.AddMessageHandler(new MessageFilterWrapper<TMessage>(handler));

    public static IServiceCollection Filter<TMessage>(
        this IServiceCollection services,
        Func<TMessage, CancellationToken, ValueTask<bool>> handler) =>
        services.AddMessageHandler(new MessageFilterWrapper<TMessage>(handler));

    public static IServiceCollection Filter<TMessage>(
        this IServiceCollection services,
        IMessageFilter<TMessage> handler) =>
        services.AddMessageHandler(handler);
}

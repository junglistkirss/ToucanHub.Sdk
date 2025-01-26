using Microsoft.Extensions.DependencyInjection.Extensions;
using Toucan.Sdk.Application.Mediator.Consumers;
using Toucan.Sdk.Application.Mediator.Internals;
using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;
using Toucan.Sdk.Shared.Messages;

namespace Toucan.Sdk.Application.Mediator;


public static class MediatorSdkApplicationModule
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        //services.TryAddScoped(_ => MediatorCommandContext.CreateEmpty());

        return services
            .AddScoped<IMediatorCommandSender, MediatorDispatcher>()
            .AddScoped<IMediatorRequestSender, MediatorDispatcher>()
            .AddScoped<IMediatorPublisher, MediatorDispatcher>()
            .AddScoped<IMediatorCommandBus, MediatorBus>()
            .AddScoped<IMediatorEventBus, MediatorBus>()
            .AddScoped<IMediatorQueryBus, MediatorBus>()
            .AddScoped<IMediatorBus, MediatorBus>();
    }

    #region Command
    public static IServiceCollection MediatorCommandMessage<TCommand, THandler>(this IServiceCollection services)
       where TCommand : CommandMessage
       where THandler : class, ICommandHandler<CommandEnvelope<TCommand>>
       => services.MediatorCommand<CommandEnvelope<TCommand>, THandler>();

    public static IServiceCollection MediatorCommand<TCommand, THandler>(this IServiceCollection services)
        where TCommand : class, ICommand
        where THandler : class, ICommandHandler<TCommand>
        => services.MediatorCommandBase<TCommand, TCommand, THandler>();

    public static IServiceCollection MediatorCommandBase<TCommand, TCommandBase, THandler>(this IServiceCollection services)
        where TCommand : class, TCommandBase
        where TCommandBase : class, ICommand
        where THandler : class, ICommandHandler<TCommandBase>
    {
        services.TryAddScoped<THandler>();
        services.TryAddScoped<IMediatorConsumer<TCommand>, MediatorCommandConsumer<TCommand, TCommandBase, THandler>>();
        return services;
    }

    public static IServiceCollection MediatorCommandMessageResponse<TCommand, THandler, TResponse>(this IServiceCollection services)
         where TCommand : CommandMessage
        where THandler : class, ICommandHandler<CommandEnvelope<TCommand>, TResponse>
        where TResponse : class
        => services.MediatorCommandResponse<CommandEnvelope<TCommand>, THandler, TResponse>();

    public static IServiceCollection MediatorCommandResponse<TCommand, THandler, TResponse>(this IServiceCollection services)
         where TCommand : class, ICommand
        where THandler : class, ICommandHandler<TCommand, TResponse>
        where TResponse : class
         => services.MediatorCommandResponseBase<TCommand, TCommand, THandler, TResponse>();

    public static IServiceCollection MediatorCommandResponseBase<TCommand, TCommandBase, THandler, TResponse>(this IServiceCollection services)
        where TCommand : class, TCommandBase, ICommand
        where TCommandBase : class, ICommand
        where THandler : class, ICommandHandler<TCommandBase, TResponse>
        where TResponse : class
    {
        services.TryAddScoped<THandler>();
        services.TryAddScoped<IMediatorConsumer<TCommand, TResponse>, MediatorCommandResponseConsumer<TCommand, TCommandBase, TResponse, THandler>>();
        return services;
    }
    #endregion

    #region Event
    public static IServiceCollection MediatorEventMessage<TEvent, THandler>(this IServiceCollection services)
        where TEvent : EventMessage
        where THandler : class, IEventHandler<EventEnvelope<TEvent>>
        => services.MediatorEvent<EventEnvelope<TEvent>, THandler>();

    public static IServiceCollection MediatorEvent<TEvent, THandler>(this IServiceCollection services)
        where TEvent : class, IEvent
        where THandler : class, IEventHandler<TEvent>
        => services.MediatorEventBase<TEvent, TEvent, THandler>();

    public static IServiceCollection MediatorEventBase<TEvent, TEventBase, THandler>(this IServiceCollection services)
        where TEvent : class, TEventBase, IEvent
        where TEventBase : class, IEvent
        where THandler : class, IEventHandler<TEventBase>
    {
        services.TryAddScoped<THandler>();
        services.TryAddScoped<IMediatorConsumer<TEvent>, MediatorEventConsumer<TEvent, TEventBase, THandler>>();
        return services;
    }
    #endregion

    #region Query
    public static IServiceCollection MediatorQueryMessage<TQuery, THandler, TResponse>(this IServiceCollection services)
        where TQuery : QueryMessage
        where THandler : class, IQueryHandler<QueryEnvelope<TQuery>, TResponse>
        where TResponse : class
        => services.MediatorQuery<QueryEnvelope<TQuery>, THandler, TResponse>();
    public static IServiceCollection MediatorQuery<TQuery, THandler, TResponse>(this IServiceCollection services)
        where TQuery : class, IQuery
        where THandler : class, IQueryHandler<TQuery, TResponse>
        where TResponse : class
        => services.MediatorQueryBase<TQuery, TQuery, THandler, TResponse>();
    public static IServiceCollection MediatorQueryBase<TQuery, TQueryBase, THandler, TResponse>(this IServiceCollection services)
        where TQuery : class, TQueryBase, IQuery
        where TQueryBase : class, IQuery
        where THandler : class, IQueryHandler<TQueryBase, TResponse>
        where TResponse : class
    {
        services.TryAddScoped<THandler>();
        services.TryAddScoped<IMediatorConsumer<TQuery, TResponse>, MediatorQueryResponseConsumer<TQuery, TQueryBase, THandler, TResponse>>();
        return services;
    }
    #endregion
}


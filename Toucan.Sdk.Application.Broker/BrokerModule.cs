using Microsoft.Extensions.DependencyInjection;
using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;
using Toucan.Sdk.Shared.Messages;

namespace Toucan.Sdk.Application.Broker;

public static class BrokerModule
{
    public static IServiceCollection AddBrokerBase(this IServiceCollection services)
    {

        return services
            .AddScoped<IBrokerCommandSender, BrokerDispatcher>()
            .AddScoped<IBrokerRequestSender, BrokerDispatcher>()
            .AddScoped<IBrokerPublisher, BrokerDispatcher>();
    }


    #region Command
    public static IBrokerServiceRegistrar CommandMessage<TCommand, THandler>(this IBrokerServiceRegistrar services)
       where TCommand : CommandMessage
       where THandler : class, ICommandHandler<CommandEnvelope<TCommand>>
       => services.Command<CommandEnvelope<TCommand>, THandler>();

    public static IBrokerServiceRegistrar Command<TCommand, THandler>(this IBrokerServiceRegistrar services)
        where TCommand : class, ICommand
        where THandler : class, ICommandHandler<TCommand>
        => services.CommandBase<TCommand, TCommand, THandler>();

    public static IBrokerServiceRegistrar CommandBase<TCommand, TCommandBase, THandler>(this IBrokerServiceRegistrar services)
        where TCommand : class, TCommandBase
        where TCommandBase : class, ICommand
        where THandler : class, ICommandHandler<TCommandBase>
    {
        services.RegisterConsumerCommandBase<TCommand, TCommandBase, THandler>();
        return services;
    }

    public static IBrokerServiceRegistrar CommandMessageResponse<TCommand, THandler, TResponse>(this IBrokerServiceRegistrar services)
         where TCommand : CommandMessage
        where THandler : class, ICommandHandler<CommandEnvelope<TCommand>, TResponse>
        where TResponse : class
        => services.CommandResponse<CommandEnvelope<TCommand>, THandler, TResponse>();

    public static IBrokerServiceRegistrar CommandResponse<TCommand, THandler, TResponse>(this IBrokerServiceRegistrar services)
         where TCommand : class, ICommand
        where THandler : class, ICommandHandler<TCommand, TResponse>
        where TResponse : class
         => services.CommandResponseBase<TCommand, TCommand, THandler, TResponse>();

    public static IBrokerServiceRegistrar CommandResponseBase<TCommand, TCommandBase, THandler, TResponse>(this IBrokerServiceRegistrar services)
        where TCommand : class, TCommandBase, ICommand
        where TCommandBase : class, ICommand
        where THandler : class, ICommandHandler<TCommandBase, TResponse>
        where TResponse : class
    {
        services.RegisterConsumerCommandResponseBase<TCommand, TCommandBase, THandler, TResponse>();
        return services;
    }
    #endregion

    #region Event
    public static IBrokerServiceRegistrar EventMessage<TEvent, THandler>(this IBrokerServiceRegistrar services)
        where TEvent : EventMessage
        where THandler : class, IEventHandler<EventEnvelope<TEvent>>
        => services.Event<EventEnvelope<TEvent>, THandler>();

    public static IBrokerServiceRegistrar Event<TEvent, THandler>(this IBrokerServiceRegistrar services)
        where TEvent : class, IEvent
        where THandler : class, IEventHandler<TEvent>
        => services.EventBase<TEvent, TEvent, THandler>();

    public static IBrokerServiceRegistrar EventBase<TEvent, TEventBase, THandler>(this IBrokerServiceRegistrar services)
        where TEvent : class, TEventBase, IEvent
        where TEventBase : class, IEvent
        where THandler : class, IEventHandler<TEventBase>
    {
        services.RegisterConsumerEventBase<TEvent, TEventBase, THandler>();
        return services;
    }
    #endregion

    #region Query
    public static IBrokerServiceRegistrar QueryMessage<TQuery, THandler, TResponse>(this IBrokerServiceRegistrar services)
        where TQuery : QueryMessage
        where THandler : class, IQueryHandler<QueryEnvelope<TQuery>, TResponse>
        where TResponse : class
        => services.Query<QueryEnvelope<TQuery>, THandler, TResponse>();
    public static IBrokerServiceRegistrar Query<TQuery, THandler, TResponse>(this IBrokerServiceRegistrar services)
        where TQuery : class, IQuery
        where THandler : class, IQueryHandler<TQuery, TResponse>
        where TResponse : class
        => services.QueryBase<TQuery, TQuery, THandler, TResponse>();
    public static IBrokerServiceRegistrar QueryBase<TQuery, TQueryBase, THandler, TResponse>(this IBrokerServiceRegistrar services)
        where TQuery : class, TQueryBase, IQuery
        where TQueryBase : class, IQuery
        where THandler : class, IQueryHandler<TQueryBase, TResponse>
        where TResponse : class
    {
        services.RegisterConsumerQueryBase<TQuery, TQueryBase, THandler, TResponse>();
        return services;
    }
    #endregion



}


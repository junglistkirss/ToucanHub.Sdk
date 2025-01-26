using Toucan.Sdk.Infrastructure.Handlers;
using Toucan.Sdk.Infrastructure.Markers;

namespace Toucan.Sdk.Application.Broker;

public interface IBrokerServiceRegistrar
{
    #region Command
    void RegisterConsumerCommandBase<TCommand, TCommandBase, THandler>()
        where TCommand : class, TCommandBase
        where TCommandBase : class, ICommand
        where THandler : class, ICommandHandler<TCommandBase>;


    void RegisterConsumerCommandResponseBase<TCommand, TCommandBase, THandler, TResponse>()
        where TCommand : class, TCommandBase, ICommand
        where TCommandBase : class, ICommand
        where THandler : class, ICommandHandler<TCommandBase, TResponse>
        where TResponse : class;
    #endregion

    #region Event
    void RegisterConsumerEventBase<TEvent, TEventBase, THandler>()
        where TEvent : class, TEventBase, IEvent
        where TEventBase : class, IEvent
        where THandler : class, IEventHandler<TEventBase>;
    #endregion

    #region Query
    void RegisterConsumerQueryBase<TQuery, TQueryBase, THandler, TResponse>()
        where TQuery : class, TQueryBase, IQuery
        where TQueryBase : class, IQuery
        where THandler : class, IQueryHandler<TQueryBase, TResponse>
        where TResponse : class;
    #endregion
}


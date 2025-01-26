namespace Toucan.Sdk.Store.Services;

public interface IContextQueryHelper<TContext> : IContextQueryModelHelper<TContext>, IContextQueryCollectionHelper<TContext>
    where TContext : IReadContextProxy
{ }

//public interface IPersistenceManager<TContext> : IModelPersistenceManager<TContext>, IBatchPersistenceManager<TContext>
//{ }

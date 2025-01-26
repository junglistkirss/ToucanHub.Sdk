namespace Toucan.Sdk.Store.QueryOptions;
public delegate Expression<Func<TEntity, bool>>? FilterHandler<TEntity, TFilter>(TFilter options)
where TEntity : class;
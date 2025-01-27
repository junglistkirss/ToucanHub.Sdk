namespace Toucan.Sdk.Store.Services;

public interface IContextWrapper<TContext>
    where TContext : IReadContextProxy
{
    public TContext Context { get; }

    //Expression<Func<TEntity, bool>>? ResolvePredicateAsync<TEntity, TFilter, TSearch>(params TFilter?[] options)
    //    where TFilter : BaseFilterNode<TSearch>
    //    where TEntity : class;

}

using Toucan.Sdk.Store.QueryOptions;

namespace Toucan.Sdk.Store.Services;


public interface IGlobalFilter<TContext, TEntity>
    where TEntity : class
{
    ValueTask<Expression<Func<TEntity, bool>>?> ApplyAsync(TContext context, CancellationToken cancellationToken);
}


public interface IFilterParser<TContext, TEntity, TOptions>
    where TEntity : class
{
    ValueTask<Expression<Func<TEntity, bool>>?> ParseAsync(TContext context, TOptions options, CancellationToken cancellationToken);
}

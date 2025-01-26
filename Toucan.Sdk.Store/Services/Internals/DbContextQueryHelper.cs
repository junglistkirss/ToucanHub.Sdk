using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Query.Filters;
using Toucan.Sdk.Contracts.Query.Page;
using Toucan.Sdk.Store.Extensions;
using Toucan.Sdk.Store.QueryOptions;

namespace Toucan.Sdk.Store.Services.Internals;

internal abstract class BaseDbContextHelper<TProxy, TContext>(
    TContext context,
    IServiceProvider serviceProvider,
    ILogger logger) : IContextWrapper<TProxy>
    where TProxy : IReadContextProxy
    where TContext : DbContext, TProxy
{
    public TProxy Context => context;
    protected IServiceProvider ServiceProvider => serviceProvider;

    protected async ValueTask<Expression<Func<TEntity, bool>>?> ResolveGlobalPredicateAsync<TEntity>(CancellationToken cancellationToken)
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? exp = null;
        IEnumerable<IGlobalFilter<TProxy, TEntity>>? services = serviceProvider.GetServices<IGlobalFilter<TProxy, TEntity>>();
        if (services?.Any() ?? false)
            foreach (var service in services)
            {
                Expression<Func<TEntity, bool>>? combine = await service.ApplyAsync(context, cancellationToken);
                if (combine is not null)
                    exp = exp.AndAlso(combine);

            }
        return exp;
    }

    protected async ValueTask<Expression<Func<TEntity, bool>>?> ResolvePredicateAsync<TEntity, TFilter, TSearch>(TFilter filter, CancellationToken cancellationToken)
        where TFilter : BaseFilterNode<TSearch>
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? exp = null;
        if (filter is not null)
        {
            logger.LogDebug("Resolving {filter} in {context} for {entity}", typeof(TFilter).Name, typeof(TProxy).Name, typeof(TEntity).Name);
            IEnumerable<IFilterParser<TProxy, TEntity, TSearch>>? services = serviceProvider.GetServices<IFilterParser<TProxy, TEntity, TSearch>>();
            if (services?.Any() ?? false)
            {
                foreach (IFilterParser<TProxy, TEntity, TSearch> service in services)
                {
                    Expression<Func<TEntity, bool>>? combine = await FilterAggragateAsync(service, filter, context, cancellationToken);
                    if (combine is not null)
                    {
                        logger.LogDebug("Return {combine}", combine);
                        exp = exp.AndAlso(combine);
                    }
                    else
                    {
                        logger.LogWarning($"Return no expression");
                    }
                }
            }
            else
            {
                logger.LogWarning("No filter found: {filter} in {context} for {entity}", typeof(TFilter).Name, typeof(TContext).Name, typeof(TEntity).Name);
            }
        }
        return exp;
    }

    private static async ValueTask<Expression<Func<TEntity, bool>>?> FilterAggragateAsync<TEntity, TFilter, TSearch>(IFilterParser<TProxy, TEntity, TSearch> service, TFilter filter, TContext dbContext, CancellationToken cancellationToken)
        where TFilter : BaseFilterNode<TSearch>
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(service);

        if (filter is null)
            return null;

        if (filter is IFilterNode<TSearch> single && single.Filter != null)
            return await service.ParseAsync(dbContext, single.Filter, cancellationToken);

        if (filter is IFilterGroup<TSearch, TFilter> group && group.Nodes != null)
        {

            List<Expression<Func<TEntity, bool>>> expressions = [];
            foreach (TFilter? node in group.Nodes)
            {
                Expression<Func<TEntity, bool>>? e = await FilterAggragateAsync(service, node, dbContext, cancellationToken);
                if (e is not null)
                    expressions.Add(e);
            }
            return group.Aggregator switch
            {
                FilterAggregator.OR => default(Expression<Func<TEntity, bool>>).OrElseNext([.. expressions]),
                _ => default(Expression<Func<TEntity, bool>>).AndAlsoNext([.. expressions])
            };
        }

        return null;
    }

}

internal sealed class DbContextQueryHelper<TProxy, TContext> : BaseDbContextHelper<TProxy, TContext>, IContextQueryHelper<TProxy>
    where TProxy : IReadContextProxy
    where TContext : DbContext, TProxy
{
    private readonly ILogger<DbContextQueryHelper<TProxy, TContext>> logger;

    public DbContextQueryHelper(
        TContext context,
        IServiceProvider serviceProvider,
        ILogger<DbContextQueryHelper<TProxy, TContext>> logger
    ) : base(context, serviceProvider, logger)
    {
        this.logger = logger;
    }

    public async ValueTask<PartialCollection<TEntity>?> FetchCollectionAsync<TEntity, TSort>(
       EntityQuerySelector<TProxy, TEntity> selector,
       IPaginatedSet<TSort>? q,
       CancellationToken cancellationToken = default
   )
       where TSort : notnull
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(selector);

        IQueryable<TEntity>? query = selector(Context);

        if (query is null)
            return new PartialCollection<TEntity>(0, []);

        Expression<Func<TEntity, bool>>? filterExpression = await ResolveGlobalPredicateAsync<TEntity>(cancellationToken);
        if (filterExpression is not null)
            query = query.Where(filterExpression);

        long count = await query.LongCountAsync(cancellationToken);

        IQueryable<TEntity> pageQuery = query;



        if (q?.SortOptions is not null)
        {
            IEnumerable<SortMarshaller<TEntity>>? orders = ResolveSorter<TEntity, TSort>([.. q.SortOptions]);
            if (orders is not null)
                pageQuery = pageQuery.SortBy(orders);
        }

        pageQuery = pageQuery.Paginate(q?.Pagination ?? Pagination.Default);

        List<TEntity> concrete = await pageQuery.ToListAsync(cancellationToken);

        return new PartialCollection<TEntity>(count, concrete);
    }

    public async ValueTask<PartialCollection<TEntity>?> FetchCollectionAsync<TEntity, TFilter, TSearch, TSort>(
        EntityQuerySelector<TProxy, TEntity> selector,
        ICollectionQuery<TFilter, TSearch, TSort>? q,
        CancellationToken cancellationToken = default
    )
        where TFilter : BaseFilterNode<TSearch>
        where TSort : notnull
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(selector);

        IQueryable<TEntity>? query = selector(Context);

        if (query is null)
            return new PartialCollection<TEntity>(0, []);

        Expression<Func<TEntity, bool>>? filterExpression = await ResolveGlobalPredicateAsync<TEntity>(cancellationToken);
        if (q?.Filter is not null)
        {
            Expression<Func<TEntity, bool>>? customFilter = await ResolvePredicateAsync<TEntity, TFilter, TSearch>(q.Filter, cancellationToken);
            if (customFilter is not null)
                filterExpression = filterExpression.AndAlso(customFilter);
        }
        if (filterExpression is not null)
            query = query.Where(filterExpression);

        long count = await query.LongCountAsync(cancellationToken);

        IQueryable<TEntity> pageQuery = query;
        if (q?.SortOptions is not null)
        {
            IEnumerable<SortMarshaller<TEntity>>? orders = ResolveSorter<TEntity, TSort>([.. q.SortOptions]);
            if (orders is not null)
                pageQuery = pageQuery.SortBy(orders);
        }

        pageQuery = pageQuery.Paginate(q?.Pagination ?? Pagination.Default);

        List<TEntity> concrete = await pageQuery.ToListAsync(cancellationToken);

        return new PartialCollection<TEntity>(count, concrete);
    }

    public async ValueTask<TEntity?> FetchModelAsync<TEntity, TFilter, TSearch>(
        EntityQuerySelector<TProxy, TEntity> selector,
        IModelQuery<TFilter, TSearch>? q,
        CancellationToken cancellationToken = default
    )
        where TFilter : BaseFilterNode<TSearch>
        where TEntity : class
    {
        IQueryable<TEntity>? query = selector(Context);

        if (query is null)
            return default;

        Expression<Func<TEntity, bool>>? filterExpression = await ResolveGlobalPredicateAsync<TEntity>(cancellationToken);
        if (filterExpression is not null)
            query = query.Where(filterExpression);

        if (q?.Filter is not null)
        {
            Expression<Func<TEntity, bool>>? customFilter = await ResolvePredicateAsync<TEntity, TFilter, TSearch>(q.Filter, cancellationToken);
            if (customFilter is not null)
                filterExpression = filterExpression.AndAlso(customFilter);
        }
        if (filterExpression is not null)
            query = query.Where(filterExpression);

        TEntity? concrete = await query.SingleOrDefaultAsync(cancellationToken);

        return concrete;
    }



    private IEnumerable<SortMarshaller<TEntity>> ResolveSorter<TEntity, TSort>(params SortOption<TSort>[] option)
        where TEntity : class
        where TSort : notnull
    {
        if (option is null)
            return [];

        ISortParser<TSort, TEntity>? service = ServiceProvider.GetService<ISortParser<TSort, TEntity>>();
        if (service is not null)
            return service.Parse(option);


        logger.LogWarning("Missing sorting {SortingType} for {Entity}", typeof(TSort).Name, typeof(TEntity).Name);
        return [];

    }

}

using Microsoft.EntityFrameworkCore;
using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Query.Page;
using Toucan.Sdk.Store.QueryOptions;

namespace Toucan.Sdk.Store.Extensions;

public static class DbQueryableExtensions
{

    public static async Task<PartialCollection<T>> ResolveDbCollectionAsync<T>(
        this DbSet<T> source,
        Pagination pagination,
        Expression<Func<T, bool>>? filter = null,
        IEnumerable<SortMarshaller<T>>? sort = null,
        CancellationToken ct = default
    )
        where T : class
        => await source.ResolveDbCollectionAsync(pagination, filter, sort, null, QueryExecutionOptions.None, ct);
    public static async Task<T?> ResolveDbSingleAsync<T>(
       this DbSet<T> source,
       Expression<Func<T, bool>>? filter = null,
       CancellationToken ct = default
   )
       where T : class
       => await source.ResolveDbSingleAsync(filter, null, QueryExecutionOptions.None, ct);
    public static async Task<PartialCollection<T>> ResolveDbCollectionAsync<T>(
        this DbSet<T> source,
        Pagination pagination,
        Expression<Func<T, bool>>? filter = null,
        IEnumerable<SortMarshaller<T>>? sort = null,
        Expression<Func<T, T>>? projection = null,
        QueryExecutionOptions options = QueryExecutionOptions.None,
       CancellationToken ct = default
    )
        where T : class
        => await source.AsQueryable().ResolveDbCollectionAsync(pagination, filter, sort, projection, options, ct);

    public static async Task<PartialCollection<T>> ResolveDbCollectionAsync<T>(
        this IQueryable<T> query,
        Pagination pagination,
        Expression<Func<T, bool>>? filter = null,
        IEnumerable<SortMarshaller<T>>? sort = null,
        Expression<Func<T, T>>? projection = null,
        QueryExecutionOptions options = QueryExecutionOptions.None,
       CancellationToken ct = default
    )
        where T : class
    {
        try
        {
            if (options.HasFlag(QueryExecutionOptions.DisableTracking)) // && context is DbContext
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            long count = await query.LongCountAsync(ct);

            IQueryable<T> pageQuery = query;
            if (sort != null)
                pageQuery = pageQuery.SortBy(sort);

            pageQuery = pageQuery.Paginate(pagination);

            if (projection != null)
                pageQuery = pageQuery.Select(projection);

            List<T> concrete = await pageQuery.ToListAsync(ct);

            return new PartialCollection<T>(count, concrete);
        }
        catch (Exception ex)
        {
            throw new QueryExecutionFailed($"{nameof(ResolveDbCollectionAsync)} failed, see inner exception", ex);
        }
    }
    public static async Task<T?> ResolveDbSingleAsync<T>(
            this DbSet<T> source,
           Expression<Func<T, bool>>? filter,
           Expression<Func<T, T>>? projection = null,
           QueryExecutionOptions options = QueryExecutionOptions.None,
           CancellationToken ct = default
       )
           where T : class
           => await source.AsQueryable().ResolveDbSingleAsync(filter, projection, options, ct);
    public static async Task<T?> ResolveDbSingleAsync<T>(
        this IQueryable<T> query,
       Expression<Func<T, bool>>? filter,
       Expression<Func<T, T>>? projection = null,
       QueryExecutionOptions options = QueryExecutionOptions.None,
       CancellationToken ct = default
   )
       where T : class
    {
        try
        {
            if (options.HasFlag(QueryExecutionOptions.DisableTracking)) // && context is DbContext
                query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (projection != null)
                query = query.Select(projection);

            T? concrete = await query.SingleOrDefaultAsync(ct);

            return concrete;
        }
        catch (Exception ex)
        {
            throw new QueryExecutionFailed($"{nameof(ResolveDbSingleAsync)} failed, see inner exception", ex);
        }
    }
}

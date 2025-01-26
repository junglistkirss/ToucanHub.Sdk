using Microsoft.EntityFrameworkCore;
using Toucan.Sdk.Contracts.Query;
using Toucan.Sdk.Contracts.Query.Page;
using Toucan.Sdk.Store.QueryOptions;

namespace Toucan.Sdk.Store.Extensions;

public class QueryExecutionFailed : Exception
{
    public QueryExecutionFailed()
    {
    }

    public QueryExecutionFailed(string? message) : base(message)
    {
    }

    public QueryExecutionFailed(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
public static class QueryableExtensions
{
    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, IEnumerable<SortMarshaller<T>> orderBy)
    {
        if (orderBy == null)
            return query;

        if (!orderBy.Any())
            return query;

        IOrderedQueryable<T>? q = null;
        foreach (SortMarshaller<T> marshaller in orderBy)
        {
            q = marshaller.Direction switch
            {
                SortDirection.Desc => q == null ? query.OrderByDescending(marshaller.Field) : q.ThenByDescending(marshaller.Field),
                _ => q == null ? query.OrderBy(marshaller.Field) : q.ThenBy(marshaller.Field),
            };
        }
        return q ?? query;
    }

    public static IQueryable<TData> Paginate<TData>(this IQueryable<TData> query, Pagination page)
    {
        int _offset = Math.Max(0, page.Offset);
        int _limit = Math.Max(1, page.Limit);

        return query.Skip(_offset).Take(_limit);
    }



    public static async Task<PartialCollection<T>> ResolveCollectionAsync<T>(
        this IQueryable<T> query,
        Pagination pagination,
        Expression<Func<T, bool>>? filter = null,
        IEnumerable<SortMarshaller<T>>? sort = null,
        Expression<Func<T, T>>? projection = null,
        CancellationToken ct = default
    )
    {
        try
        {
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
            throw new QueryExecutionFailed($"{nameof(ResolveCollectionAsync)} failed, see inner exception", ex);
        }
    }

    public static async Task<PartialCollection<TProjected>> ResolveCollectionAsync<T, TProjected>(
        this IQueryable<T> query,
        Pagination pagination,
        Expression<Func<T, TProjected>> projection ,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<TProjected, bool>>? filterProjected = null,
        IEnumerable<SortMarshaller<T>>? sort = null,
        IEnumerable<SortMarshaller<TProjected>>? sortProjected = null,
        CancellationToken ct = default
    )
    {
        try
        {
            if (filter != null)
                query = query.Where(filter);
            if (sort != null)
                query = query.SortBy(sort);

            IQueryable<TProjected> pQuery = query.Select(projection);
            if (filterProjected != null)
                pQuery = pQuery.Where(filterProjected);

            if (sortProjected != null)
                pQuery = pQuery.SortBy(sortProjected);


            long count = await pQuery.LongCountAsync(ct);

            pQuery = pQuery.Paginate(pagination);


            List<TProjected> concrete = await pQuery.ToListAsync(ct);

            return new PartialCollection<TProjected>(count, concrete);
        }
        catch (Exception ex)
        {
            throw new QueryExecutionFailed($"{nameof(ResolveCollectionAsync)} failed, see inner exception", ex);
        }
    }
    public static async Task<T?> ResolveSingleAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>>? filter,
        Expression<Func<T, T>>? projection = null,
        CancellationToken ct = default)
        where T : class
    {
        try
        {
            if (filter != null)
                query = query.Where(filter);

            if (projection != null)
                query = query.Select(projection);

            T? concrete = await query.SingleOrDefaultAsync(ct);

            return concrete;
        }
        catch (Exception ex)
        {
            throw new QueryExecutionFailed($"{nameof(ResolveSingleAsync)} failed, see inner exception", ex);
        }
    }
}

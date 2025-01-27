using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Toucan.Sdk.Contracts.Extensions;
using Toucan.Sdk.Contracts.Names;
using Toucan.Sdk.Contracts.Query.Filters;
using Toucan.Sdk.Contracts.Query.Filters.Abstractions;

namespace Toucan.Sdk.Store.QueryOptions;

public static class ExpressionFilterProjectorExtensions
{
    private static readonly MethodInfo? StartsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)]);
    private static readonly MethodInfo? EndsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)]);
    private static readonly MethodInfo? ContainsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);

    public static Expression<Func<T, bool>>? CreatePredicateExpression<T, TValue>(this SameFilter<TValue> filter, Expression<Func<T, TValue?>> accessor)
    {
        ConstantExpression compareValue = Expression.Constant(filter.Value);
        BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
        return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);
    }
    public static Expression<Func<T, bool>>? CreatePredicateExpression<T>(this DateFilter filter, Expression<Func<T, DateTimeOffset>> accessor)
    {
        if (accessor == null)
            return null;

        //if (accessor.Body.NodeType != ExpressionType.MemberAccess)
        //    return null;

        ConstantExpression compareValue = Expression.Constant(new DateTimeOffset(filter.Value));

        switch (filter.Method)
        {
            default:
            case DateFilterMethod.Equals:
                BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);

            case DateFilterMethod.GreaterThan:
                BinaryExpression gt = Expression.GreaterThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(gt, accessor.Parameters);

            case DateFilterMethod.GreaterThanOrEquals:
                BinaryExpression gte = Expression.GreaterThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(gte, accessor.Parameters);


            case DateFilterMethod.LessThan:
                BinaryExpression lt = Expression.LessThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lt, accessor.Parameters);

            case DateFilterMethod.LessThanOrEquals:
                BinaryExpression lte = Expression.LessThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lte, accessor.Parameters);

            case DateFilterMethod.NotEquals:
                BinaryExpression notEq = Expression.NotEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(notEq, accessor.Parameters);

            case DateFilterMethod.IsNull:
                BinaryExpression nul = Expression.Equal(accessor.Body, Expression.Constant(null));
                return Expression.Lambda<Func<T, bool>>(nul, accessor.Parameters);
        }
    }

    [Obsolete("Must check nullity")]
    public static Expression<Func<T, bool>>? CreatePredicateExpression<T>(this DateFilter filter, Expression<Func<T, DateTimeOffset?>> accessor)
    {
        if (accessor == null)
            return null;

        //if (accessor.Body.NodeType != ExpressionType.MemberAccess)
        //    return null;

        ConstantExpression compareValue = Expression.Constant(new DateTimeOffset(filter.Value));

        switch (filter.Method)
        {
            default:
            case DateFilterMethod.Equals:
                BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);

            case DateFilterMethod.GreaterThan:
                BinaryExpression gt = Expression.GreaterThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(gt, accessor.Parameters);

            case DateFilterMethod.GreaterThanOrEquals:
                BinaryExpression gte = Expression.GreaterThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(gte, accessor.Parameters);


            case DateFilterMethod.LessThan:
                BinaryExpression lt = Expression.LessThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lt, accessor.Parameters);

            case DateFilterMethod.LessThanOrEquals:
                BinaryExpression lte = Expression.LessThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lte, accessor.Parameters);

            case DateFilterMethod.NotEquals:
                BinaryExpression notEq = Expression.NotEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(notEq, accessor.Parameters);

            case DateFilterMethod.IsNull:
                BinaryExpression nul = Expression.Equal(accessor.Body, Expression.Constant(null));
                return Expression.Lambda<Func<T, bool>>(nul, accessor.Parameters);
        }
    }
    [Obsolete("Must check nullity")]
    public static Expression<Func<T, bool>>? CreatePredicateExpression<T>(this DateFilter filter, Expression<Func<T, DateTime?>> accessor)
    {
        if (accessor == null)
            return null;

        if (accessor.Body.NodeType != ExpressionType.MemberAccess)
            return null;

        ConstantExpression compareValue = Expression.Constant(filter.Value);

        switch (filter.Method)
        {
            default:
            case DateFilterMethod.Equals:
                BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);

            case DateFilterMethod.GreaterThan:
                BinaryExpression gt = Expression.GreaterThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(gt, accessor.Parameters);

            case DateFilterMethod.GreaterThanOrEquals:
                BinaryExpression gte = Expression.GreaterThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(gte, accessor.Parameters);


            case DateFilterMethod.LessThan:
                BinaryExpression lt = Expression.LessThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lt, accessor.Parameters);

            case DateFilterMethod.LessThanOrEquals:
                BinaryExpression lte = Expression.LessThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lte, accessor.Parameters);

            case DateFilterMethod.NotEquals:
                BinaryExpression notEq = Expression.NotEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(notEq, accessor.Parameters);

            case DateFilterMethod.IsNull:
                BinaryExpression nul = Expression.Equal(accessor.Body, Expression.Constant(null));
                return Expression.Lambda<Func<T, bool>>(nul, accessor.Parameters);
        }
    }

    public static Expression<Func<T, bool>>? CreatePredicateExpression<TEnum, T, TValue>(this EnumFilter<TEnum> filter, Expression<Func<T, TValue?>> accessor)
    {
        if (accessor == null)
            return null;

        ConstantExpression compareValue = Expression.Constant(filter.Value);

        switch (filter.Method)
        {
            default:
            case EnumFilterMethod.Equals:
                BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);

            case EnumFilterMethod.HasFlag:
                BinaryExpression hf = Expression.And(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(hf, accessor.Parameters);

            case EnumFilterMethod.GreaterThan:
                BinaryExpression greater = Expression.GreaterThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(greater, accessor.Parameters);

            case EnumFilterMethod.GreaterThanOrEquals:
                BinaryExpression greaterOrEq = Expression.GreaterThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(greaterOrEq, accessor.Parameters);

            case EnumFilterMethod.LessThan:
                BinaryExpression less = Expression.LessThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(less, accessor.Parameters);

            case EnumFilterMethod.LessThanOrEquals:
                BinaryExpression lessOrEq = Expression.LessThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lessOrEq, accessor.Parameters);

            case EnumFilterMethod.NotEquals:
                BinaryExpression notEq = Expression.NotEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(notEq, accessor.Parameters);

            case EnumFilterMethod.NotHasFlag:
                UnaryExpression nf = Expression.Not(Expression.And(accessor.Body, compareValue));
                return Expression.Lambda<Func<T, bool>>(nf, accessor.Parameters);
        }

    }

    [Obsolete("Must check nullity")]
    public static Expression<Func<T, bool>>? CreatePredicateExpression<T, TNumeric>(this NumericFilter<TNumeric> filter, Expression<Func<T, TNumeric?>> accessor)
        where TNumeric : unmanaged, IComparable, IComparable<TNumeric>, IConvertible, IEquatable<TNumeric>, IFormattable
    {
        if (accessor == null)
            return null;

        if (!accessor.Body.Type.IsAssignableFrom(typeof(TNumeric)))
            return null;

        ConstantExpression compareValue = Expression.Constant(filter.Value);

        switch (filter.Method)
        {
            default:
            case NumericFilterMethod.Equals:
                BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);

            case NumericFilterMethod.NotEquals:
                BinaryExpression notEq = Expression.NotEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(notEq, accessor.Parameters);

            case NumericFilterMethod.GreaterThan:
                BinaryExpression greater = Expression.GreaterThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(greater, accessor.Parameters);

            case NumericFilterMethod.GreaterThanOrEquals:
                BinaryExpression greaterOrEq = Expression.GreaterThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(greaterOrEq, accessor.Parameters);

            case NumericFilterMethod.LessThan:
                BinaryExpression less = Expression.LessThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(less, accessor.Parameters);

            case NumericFilterMethod.LessThanOrEquals:
                BinaryExpression lessOrEq = Expression.LessThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lessOrEq, accessor.Parameters);
        }
    }

    public static Expression<Func<T, bool>>? CreatePredicateExpression<T, TNumeric>(this NumericFilter<TNumeric> filter, Expression<Func<T, TNumeric>> accessor)
        where TNumeric : unmanaged, IComparable, IComparable<TNumeric>, IConvertible, IEquatable<TNumeric>, IFormattable
    {
        if (accessor == null)
            return null;

        if (!accessor.Body.Type.IsAssignableFrom(typeof(TNumeric)))
            return null;

        ConstantExpression compareValue = Expression.Constant(filter.Value);

        switch (filter.Method)
        {
            default:
            case NumericFilterMethod.Equals:
                BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);

            case NumericFilterMethod.NotEquals:
                BinaryExpression notEq = Expression.NotEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(notEq, accessor.Parameters);

            case NumericFilterMethod.GreaterThan:
                BinaryExpression greater = Expression.GreaterThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(greater, accessor.Parameters);

            case NumericFilterMethod.GreaterThanOrEquals:
                BinaryExpression greaterOrEq = Expression.GreaterThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(greaterOrEq, accessor.Parameters);

            case NumericFilterMethod.LessThan:
                BinaryExpression less = Expression.LessThan(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(less, accessor.Parameters);

            case NumericFilterMethod.LessThanOrEquals:
                BinaryExpression lessOrEq = Expression.LessThanOrEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(lessOrEq, accessor.Parameters);
        }
    }


    private static Expression<Func<T, bool>>? CreatePredicateStringExpression<T>(Expression<Func<T, string?>>? accessor, StringFilterMethod method, string? value)
    {
        if (accessor == null)
            return null;

        //if (accessor.Body.NodeType != ExpressionType.MemberAccess)
        //    return null;

        if (!typeof(string).IsAssignableFrom(accessor.Body.Type))
            return null;

        ConstantExpression compareValue = Expression.Constant(value);

        switch (method)
        {
            default:
            case StringFilterMethod.Equals:
                if (string.IsNullOrEmpty(value))
                    return null;
                BinaryExpression eq = Expression.Equal(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);

            case StringFilterMethod.StartsWith:
                if (StartsWithMethod == null || string.IsNullOrEmpty(value))
                    return null;
                MethodCallExpression start = Expression.Call(accessor.Body, StartsWithMethod, compareValue);
                return Expression.Lambda<Func<T, bool>>(start, accessor.Parameters);

            case StringFilterMethod.EndsWith:
                if (EndsWithMethod == null || string.IsNullOrEmpty(value))
                    return null;
                MethodCallExpression end = Expression.Call(accessor.Body, EndsWithMethod, compareValue);
                return Expression.Lambda<Func<T, bool>>(end, accessor.Parameters);

            case StringFilterMethod.Contains:
                if (ContainsMethod == null || string.IsNullOrEmpty(value))
                    return null;
                MethodCallExpression contain = Expression.Call(accessor.Body, ContainsMethod, compareValue);
                return Expression.Lambda<Func<T, bool>>(contain, accessor.Parameters);

            case StringFilterMethod.NotEquals:
                if (string.IsNullOrEmpty(value))
                    return null;
                BinaryExpression notEq = Expression.NotEqual(accessor.Body, compareValue);
                return Expression.Lambda<Func<T, bool>>(notEq, accessor.Parameters);

            case StringFilterMethod.IsNull:
                BinaryExpression nul = Expression.Equal(accessor.Body, Expression.Constant(null));
                return Expression.Lambda<Func<T, bool>>(nul, accessor.Parameters);

            case StringFilterMethod.IsEmpty:
                BinaryExpression empty = Expression.Equal(accessor.Body, Expression.Constant(string.Empty));
                return Expression.Lambda<Func<T, bool>>(empty, accessor.Parameters);
        }
    }

    public static bool IsValid([NotNullWhen(true)] this StringFilter? filter)
    {
        return filter is not null
            && (
                (!string.IsNullOrWhiteSpace(filter.Value) && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }

    public static bool IsValid([NotNullWhen(true)] this StringifyFilter<Slug>? filter)
    {
        return filter is not null
            && (
                (!filter.Value.IsEmpty() && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }

    public static bool IsValid([NotNullWhen(true)] this StringifyFilter<DomainId>? filter)
    {
        return filter is not null
            && (
                (filter.Value != DomainId.Empty && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }


    public static bool IsValid([NotNullWhen(true)] this StringifyFilter<RefToken>? filter)
    {
        return filter is not null
            && (
                (!filter.Value.IsEmpty && filter.Method != StringFilterMethod.IsNull)
                || (filter.Method == StringFilterMethod.IsNull)
            );
    }


    public static bool IsValid([NotNullWhen(true)] this DateFilter? filter)
    {
        return filter is not null
            && (
                (filter.Value >= DateTime.MinValue && filter.Value <= DateTime.MaxValue && filter.Method != DateFilterMethod.IsNull)
                || (filter.Method == DateFilterMethod.IsNull)
            );
    }

    public static bool IsValid([NotNullWhen(true)] this NumericFilter<double>? filter)
    {
        return filter is not null
            && filter.Value != double.NaN;
    }

    public static bool IsValid([NotNullWhen(true)] this NumericFilter<long>? filter)
    {
        return filter is not null
            && filter.Value >= long.MinValue
            && filter.Value <= long.MaxValue;
    }

    public static bool IsValid([NotNullWhen(true)] this ExistsFilter<DomainId>? filter)
    {
        return filter is not null
            && filter.Value?.Length > 0;
    }

    public static Expression<Func<T, bool>>? CreatePredicateExpression<T>(this StringFilter filter, Expression<Func<T, string?>> accessor)
    {
        return CreatePredicateStringExpression(accessor, filter.Method, filter.Value);
    }

    public static Expression<Func<T, bool>>? CreatePredicateExpression<T, TValue>(this StringifyFilter<TValue> filter, Expression<Func<T, string?>> accessor)
    {
        return CreatePredicateStringExpression(accessor, filter.Method, filter.Value?.ToString());

    }


    public delegate Expression<Func<T, bool>>? ExpressionFilterProjector<TFilter, T>(TFilter filter) where TFilter : class, IFilter<TFilter>;


    public static Expression<Func<T, bool>>? Project<TSubFilter, T, TItem>(
        this CollectionFilter<TSubFilter> filter,
        Expression<Func<T, ICollection<TItem>>> accessor,
        ExpressionFilterProjector<TSubFilter, TItem> subProjector
    )
        where TSubFilter : class, IFilter<TSubFilter>
    {
        ArgumentNullException.ThrowIfNull(accessor);

        Expression<Func<T, bool>>? result = null;
        if (filter.Value is not null)
            foreach (TSubFilter item in filter.Value)
            {
                Expression<Func<TItem, bool>>? subExp = subProjector(item);
                if (subExp is not null)
                {
                    MethodCallExpression anyMethodCallExpression = Expression.Call(
                        typeof(Enumerable),
                        "Any",
                        [typeof(TItem)],
                        accessor.Body,
                        Expression.Lambda(subExp.Body, subExp.Parameters[0])
                    );

                    Expression<Func<T, bool>> lambdaExpression = Expression.Lambda<Func<T, bool>>(
                        filter.Method == CollectionFilterMethod.Any
                            ? Expression.OrElse(result?.Body ?? Expression.Constant(false), anyMethodCallExpression)
                            : Expression.AndAlso(result?.Body ?? Expression.Constant(true), anyMethodCallExpression),
                        accessor.Parameters[0]
                    );

                    result = lambdaExpression;
                }
            }
        return result;
    }
}


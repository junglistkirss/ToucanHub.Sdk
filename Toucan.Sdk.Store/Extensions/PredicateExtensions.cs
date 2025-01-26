namespace Toucan.Sdk.Store.Extensions;

public delegate Expression<Func<T, bool>>? ExpressionOperator<T>(Expression<Func<T, bool>>? leftExpression, Expression<Func<T, bool>>? rightExpression);
public delegate Expression<Func<T, bool>>? ExpressionListOperator<T>(Expression<Func<T, bool>>? first, params Expression<Func<T, bool>>[] nextExpressions);

//    public static class ExpressionOperators<T>
//    {
//#pragma warning disable CA2211 // Les champs non constants ne doivent pas �tre visibles
//        public static ExpressionOperator<T> OR = PredicateExtensions.OrElse;
//        public static ExpressionOperator<T> AND = PredicateExtensions.AndAlso;

//        public static ExpressionListOperator<T> ANY = PredicateExtensions.OrElseNext;
//        public static ExpressionListOperator<T> ALL = PredicateExtensions.AndAlsoNext;
//#pragma warning restore CA2211 // Les champs non constants ne doivent pas �tre visibles
//    }
public static class PredicateExtensions
{
    //public static Expression<Func<T, bool>> ApplyOperator<T>(this Expression<Func<T, bool>> left, ExpressionOperator<T> expressionOperator, Expression<Func<T, bool>> right)
    //       => expressionOperator(left, right);
    //public static Expression<Func<T, bool>> ApplyListOperator<T>(this Expression<Func<T, bool>> left, ExpressionListOperator<T> expressionOperator, params Expression<Func<T, bool>>[] rights)
    //    => expressionOperator(left, rights);

    public static Expression<Func<T, bool>>? RawEquals<T, TValue>(this TValue value, params Expression<Func<T, TValue>>[] accessors)
                where TValue : IComparable, IComparable<TValue>, IEquatable<TValue>
    {
        if (accessors != null && accessors.Length > 0)
        {
            Expression<Func<T, bool>>? predicate = null;
            foreach (Expression<Func<T, TValue>> accessor in accessors)
            {
                ConstantExpression _right = Expression.Constant(value);
                BinaryExpression eq = Expression.Equal(accessor.Body, _right);
                Expression<Func<T, bool>> exp = Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);
                predicate = predicate.AndAlso(exp);
            }
            return predicate;
        }
        return null;
    }

    public static Expression<Func<T, bool>> IsNull<T, TValue>(Expression<Func<T, TValue>> accessor)
    {
        ConstantExpression _right = Expression.Constant(null);
        BinaryExpression eq = Expression.Equal(accessor.Body, _right);
        return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);
    }

    public static Expression<Func<T, bool>> IsNotNull<T, TValue>(this Expression<Func<T, TValue>> accessor)
    {
        ConstantExpression _right = Expression.Constant(null);
        BinaryExpression eq = Expression.NotEqual(accessor.Body, _right);
        return Expression.Lambda<Func<T, bool>>(eq, accessor.Parameters);
    }

    //public static Expression<Func<T, TOut>> ExtendSelector<T, TBase, TOut>(this Expression<Func<T, TBase>> baseMember, Expression<Func<TBase, TOut>> selector)
    //{
    //    if (selector != null
    //        && baseMember != null
    //        && baseMember.Body is MemberExpression baseMemberExpression
    //        && selector.Body is MemberExpression extendMemberExpression)
    //    {
    //        return Expression.Lambda<Func<T, TOut>>(Expression.MakeMemberAccess(baseMemberExpression, extendMemberExpression.Member), baseMember.Parameters[0]);
    //    }
    //    return null;
    //}

    public static Expression<Func<T, bool>> True<T>() => f => true;
    public static Expression<Func<T, bool>> False<T>() => f => false;

    public static Expression<Func<T, bool>>? AndAlso<T>(this Expression<Func<T, bool>>? leftExpression,
        Expression<Func<T, bool>>? rightExpression)
    {
        if (leftExpression == null)
            return rightExpression;
        if (rightExpression == null)
            return leftExpression;
        return Combine(leftExpression, rightExpression, Expression.AndAlso);
    }

    public static Expression<Func<T, bool>>? OrElse<T>(this Expression<Func<T, bool>>? leftExpression,
        Expression<Func<T, bool>>? rightExpression)
    {
        if (leftExpression == null)
            return rightExpression;
        if (rightExpression == null)
            return leftExpression;
        return Combine(leftExpression, rightExpression, Expression.OrElse);
    }

    public static Expression<Func<T, bool>>? OrElseNext<T>(this Expression<Func<T, bool>>? first, params Expression<Func<T, bool>>?[] nextExpressions)
    {
        Expression<Func<T, bool>>? exp = first;

        if (nextExpressions != null && nextExpressions.Length > 0)
        {
            int i = 0;
            do
            {
                exp = exp.OrElse(nextExpressions[i]);
                i += 1;
            } while (i < nextExpressions.Length);
        }

        return exp;
    }
    public static Expression<Func<T, bool>>? AndAlsoNext<T>(this Expression<Func<T, bool>>? first, params Expression<Func<T, bool>>?[] nextExpressions)
    {
        Expression<Func<T, bool>>? exp = first;

        if (nextExpressions != null && nextExpressions.Length > 0)
        {
            int i = 0;
            do
            {
                exp = exp.AndAlso(nextExpressions[i]);
                i += 1;
            } while (i < nextExpressions.Length);
        }

        return exp;
    }

    private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression, Func<Expression, Expression, BinaryExpression> combineOperator)
    {
        ParameterExpression leftParameter = leftExpression.Parameters[0];
        ParameterExpression rightParameter = rightExpression.Parameters[0];

        ReplaceParameterVisitor visitor = new(rightParameter, leftParameter);

        Expression leftBody = leftExpression.Body;
        Expression rightBody = visitor.Visit(rightExpression.Body);

        return Expression.Lambda<Func<T, bool>>(combineOperator(leftBody, rightBody), leftParameter);
    }

    private class ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => ReferenceEquals(node, oldParameter) ? newParameter : base.VisitParameter(node);
    }
}

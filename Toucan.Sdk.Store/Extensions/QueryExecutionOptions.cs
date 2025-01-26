namespace Toucan.Sdk.Store.Extensions;

public delegate IQueryable<T>? EntityQuerySelector<TContext, T>(TContext context);

// public delegate ICollection<T>? EntitySourceSelector<TContext, T>(TContext context);
// public delegate T EntityBuilder<T>(T? input);
// public delegate T[] EntityBatchBuilder<T>(ICollection<T> source);


[Flags]
public enum QueryExecutionOptions
{
    None = 0,
    DisableTracking = 1,
}

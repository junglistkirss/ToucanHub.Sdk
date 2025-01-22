namespace Toucan.Sdk.Pipeline;

public delegate T MiddlewareFactory<T>(IServiceProvider provider);
public delegate T MiddlewareKeyFactory<T, TKey>(IServiceProvider provider, TKey key);

public delegate T MiddlewareProvide<T>();
public delegate T MiddlewareKeyProvide<T, TKey>(TKey key);


#region NextStepDelegate
public delegate void RichNextDelegate<TContext>(TContext context) where TContext : IPipelineContext;
public delegate ValueTask RichNextAsyncDelegate<TContext>(TContext context) where TContext : IPipelineContext;

public delegate void NextDelegate();
public delegate ValueTask NextAsyncDelegate();
#endregion

#region HandleDelegate
// Handle and call next
public delegate void RichMiddlewareHandle<TContext>(TContext context, RichNextDelegate<TContext> next) where TContext : IPipelineContext;
public delegate ValueTask AsyncRichMiddlewareHandle<TContext>(TContext context, RichNextAsyncDelegate<TContext> next) where TContext : IPipelineContext;

// Handle and forward
public delegate void MiddlewareHandle<TContext>(TContext context, NextDelegate next) where TContext : IPipelineContext;
public delegate ValueTask AsyncMiddlewareHandle<TContext>(TContext context, NextAsyncDelegate next) where TContext : IPipelineContext;

// Action Only
public delegate void MiddlewareActionHandle<TContext>(TContext context) where TContext : IPipelineContext;
public delegate ValueTask AsyncMiddlewareActionHandle<TContext>(TContext context) where TContext : IPipelineContext;
#endregion

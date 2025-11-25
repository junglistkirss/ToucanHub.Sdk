namespace ToucanHub.Sdk.Pipeline;
/// <summary>
/// Middleware constructed using the <paramref name="serviceProvider"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="serviceProvider"></param>
/// <returns></returns>
public delegate T MiddlewareFactory<T>(IServiceProvider serviceProvider);

/// <summary>
/// Middleware constructed using the <paramref name="serviceProvider"/> and given <paramref name="serviceKey"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="serviceProvider"></param>
/// <returns></returns>
public delegate T MiddlewareKeyFactory<T, TKey>(IServiceProvider serviceProvider, TKey serviceKey);

/// <summary>
/// Construct a middleware
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
public delegate T MiddlewareProvide<T>();

/// <summary>
/// Construct a middleware using a given <paramref name="key"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
public delegate T MiddlewareKeyProvide<T, TKey>(TKey key);


#region NextStepDelegate
/// <summary>
/// Call next configured middleware, you should pass the <paramref name="context"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
public delegate void RichNextDelegate<TContext>(TContext context) where TContext : IPipelineContext;

/// <summary>
/// Async call next configured middleware, you should pass the <paramref name="context"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
public delegate ValueTask RichNextAsyncDelegate<TContext>(TContext context) where TContext : IPipelineContext;

/// <summary>
/// Call next configured middleware
/// </summary>
public delegate void NextDelegate();

/// <summary>
/// Async call next configured middleware
/// </summary>
public delegate ValueTask NextAsyncDelegate();
#endregion

#region HandleDelegate
/// <summary>
/// Synchronous middleware, you should call <paramref name="next"/> using the <paramref name="context"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
/// <param name="next"></param>
public delegate void RichMiddlewareHandle<TContext>(TContext context, RichNextDelegate<TContext> next) where TContext : IPipelineContext;

/// <summary>
/// Asynchronous middleware, you should call <paramref name="next"/> using the <paramref name="context"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
/// <param name="next"></param>
public delegate ValueTask AsyncRichMiddlewareHandle<TContext>(TContext context, RichNextAsyncDelegate<TContext> next) where TContext : IPipelineContext;

/// <summary>
/// Synchronous middleware, you should call <paramref name="next"/> without passing the <paramref name="context"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
/// <param name="next"></param>
public delegate void MiddlewareHandle<TContext>(TContext context, NextDelegate next) where TContext : IPipelineContext;

/// <summary>
/// Asynchronous middleware, you should call <paramref name="next"/> without passing the <paramref name="context"/>
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
/// <param name="next"></param>
public delegate ValueTask AsyncMiddlewareHandle<TContext>(TContext context, NextAsyncDelegate next) where TContext : IPipelineContext;

/// <summary>
/// Synchronous termination, next middleware is never called
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
public delegate void MiddlewareTermination<TContext>(TContext context) where TContext : IPipelineContext;

/// <summary>
/// Asynchronous termination, next middleware is never called
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <param name="context"></param>
public delegate ValueTask AsyncMiddlewareTermination<TContext>(TContext context) where TContext : IPipelineContext;
#endregion

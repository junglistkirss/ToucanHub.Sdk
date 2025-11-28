using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using ToucanHub.Sdk.Pipeline.Internal;

namespace ToucanHub.Sdk.Pipeline;

public sealed class AsyncPipelineBuilder<TContext>
        where TContext : IPipelineContext
{
    public static AsyncPipelineBuilder<TContext> CreateBuilder(ServiceLifetime serviceLifetime = ServiceLifetime.Transient) => new(serviceLifetime);

    private AsyncPipelineBuilder(ServiceLifetime serviceLifetime)
    {
        this.serviceLifetime = serviceLifetime;
    }

    private readonly List<ServiceDescriptor> descriptors = [];
    private readonly ServiceLifetime serviceLifetime;



    public AsyncPipelineBuilder<TContext> Use<TBehavior>(ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IAsyncPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), typeof(TBehavior), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Use<TBehavior>(Func<IServiceProvider, TBehavior> factory, ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IAsyncPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), factory, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Use<TBehavior>(Func<TBehavior> factory, ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IAsyncPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => factory(), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Use<TBehavior>(TBehavior instance, ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IAsyncPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => instance, behaviorLifetime ?? serviceLifetime));
        return this;
    }



    #region Delegates

    public AsyncPipelineBuilder<TContext> ThenAsync(AsyncMiddlewareHandle<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineBehaviorHandle<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> ThenAsync(AsyncRichMiddlewareHandle<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineBehavior<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> TerminateAsync(AsyncMiddlewareAction<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineBehaviorTermination<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> ContinueAsync(AsyncMiddlewareAction<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineBehaviorContinuation<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }


    public AsyncPipelineBuilder<TContext> Then(RichMiddlewareHandle<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineSyncBehavior<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Then(MiddlewareHandle<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineBehaviorSyncHandle<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Terminate(MiddlewareAction<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineBehaviorSyncTermination<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Continue(MiddlewareAction<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (_) => new AsyncPipelineBehaviorSyncContinuation<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    #endregion

    #region Factory

    public AsyncPipelineBuilder<TContext> ThenAsync(MiddlewareFactory<AsyncMiddlewareHandle<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            AsyncMiddlewareHandle<TContext> handle = step(s);
            return new AsyncPipelineBehaviorHandle<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> ThenAsync(MiddlewareFactory<AsyncRichMiddlewareHandle<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            AsyncRichMiddlewareHandle<TContext> handle = step(s);
            return new AsyncPipelineBehavior<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> ContinueAsync(MiddlewareFactory<AsyncMiddlewareAction<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            AsyncMiddlewareAction<TContext> handle = step(s);
            return new AsyncPipelineBehaviorContinuation<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> TerminateAsync(MiddlewareFactory<AsyncMiddlewareAction<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            AsyncMiddlewareAction<TContext> handle = step(s);
            return new AsyncPipelineBehaviorTermination<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }


    public AsyncPipelineBuilder<TContext> Then(MiddlewareFactory<MiddlewareHandle<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            MiddlewareHandle<TContext> handle = step(s);
            return new AsyncPipelineBehaviorSyncHandle<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Then(MiddlewareFactory<RichMiddlewareHandle<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            RichMiddlewareHandle<TContext> handle = step(s);
            return new AsyncPipelineSyncBehavior<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Continue(MiddlewareFactory<MiddlewareAction<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            MiddlewareAction<TContext> handle = step(s);
            return new AsyncPipelineBehaviorSyncContinuation<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Terminate(MiddlewareFactory<MiddlewareAction<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            MiddlewareAction<TContext> handle = step(s);
            return new AsyncPipelineBehaviorSyncTermination<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    #endregion

    #region Dependency

    public AsyncPipelineBuilder<TContext> ThenAsync<T>(Func<T, AsyncRichMiddlewareHandle<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            AsyncRichMiddlewareHandle<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineBehavior<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> ThenAsync<T>(Func<T, AsyncMiddlewareHandle<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            AsyncMiddlewareHandle<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineBehaviorHandle<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> TerminateAsync<T>(Func<T, AsyncMiddlewareAction<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            AsyncMiddlewareAction<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineBehaviorTermination<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> ContinueAsync<T>(Func<T, AsyncMiddlewareAction<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            AsyncMiddlewareAction<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineBehaviorContinuation<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }


    public AsyncPipelineBuilder<TContext> Then<T>(Func<T, RichMiddlewareHandle<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
      where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            RichMiddlewareHandle<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineSyncBehavior<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Then<T>(Func<T, MiddlewareHandle<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            MiddlewareHandle<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineBehaviorSyncHandle<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Terminate<T>(Func<T, MiddlewareAction<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            MiddlewareAction<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineBehaviorSyncTermination<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Continue<T>(Func<T, MiddlewareAction<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IAsyncPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            MiddlewareAction<TContext> handle = handleProvider(dependency);
            return new AsyncPipelineBehaviorSyncContinuation<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    #endregion


    public IAsyncPipeline<TContext> Build(IServiceProvider serviceProvider)
    {
        return new AsyncPipeline<TContext>(serviceProvider.GetServices<IAsyncPipelineBehavior<TContext>>());
    }

    public void Register(IServiceCollection serviceDescriptors)
    {
        foreach (ServiceDescriptor item in descriptors)
            serviceDescriptors.Add(item);
    }
}

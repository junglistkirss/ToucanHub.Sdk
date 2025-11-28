using Microsoft.Extensions.DependencyInjection;
using ToucanHub.Sdk.Pipeline.Internal;

namespace ToucanHub.Sdk.Pipeline;

public sealed class PipelineBuilder<TContext>
        where TContext : IPipelineContext
{
    public static PipelineBuilder<TContext> CreateBuilder(ServiceLifetime serviceLifetime = ServiceLifetime.Transient) => new(serviceLifetime);

    private PipelineBuilder(ServiceLifetime serviceLifetime)
    {
        this.serviceLifetime = serviceLifetime;
    }

    private readonly List<ServiceDescriptor> descriptors = [];
    private readonly ServiceLifetime serviceLifetime;

    public PipelineBuilder<TContext> Use<TBehavior>(ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), typeof(TBehavior), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Use<TBehavior>(Func<IServiceProvider, TBehavior> factory, ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), factory, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Use<TBehavior>(Func<TBehavior> factory, ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (_) => factory(), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Use<TBehavior>(TBehavior instance, ServiceLifetime? behaviorLifetime = null)
        where TBehavior : class, IPipelineBehavior<TContext>
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (_) => instance, behaviorLifetime ?? serviceLifetime));
        return this;
    }


    #region Delegates

    public PipelineBuilder<TContext> Then(RichMiddlewareHandle<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (_) => new PipelineBehavior<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Then(MiddlewareHandle<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (_) => new PipelineBehaviorHandle<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Continue(MiddlewareAction<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (_) => new PipelineBehaviorContinuation<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Terminate(MiddlewareAction<TContext> handle, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (_) => new PipelineBehaviorTermination<TContext>(handle), behaviorLifetime ?? serviceLifetime));
        return this;
    }

    #endregion

    #region Dependency
    public PipelineBuilder<TContext> Then<T>(Func<T, RichMiddlewareHandle<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
        where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            RichMiddlewareHandle<TContext> handle = handleProvider(dependency);
            return new PipelineBehavior<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Then<T>(Func<T, MiddlewareHandle<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
       where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            MiddlewareHandle<TContext> handle = handleProvider(dependency);
            return new PipelineBehaviorHandle<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Terminate<T>(Func<T, MiddlewareAction<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
        where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            MiddlewareAction<TContext> handle = handleProvider(dependency);
            return new PipelineBehaviorTermination<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Continue<T>(Func<T, MiddlewareAction<TContext>> handleProvider, ServiceLifetime? behaviorLifetime = null)
        where T : class
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            T dependency = s.GetRequiredService<T>();
            MiddlewareAction<TContext> handle = handleProvider(dependency);
            return new PipelineBehaviorContinuation<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    #endregion

    #region Factory

    public PipelineBuilder<TContext> Then(MiddlewareFactory<RichMiddlewareHandle<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            RichMiddlewareHandle<TContext> handle = step(s);
            return new PipelineBehavior<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Then(MiddlewareFactory<MiddlewareHandle<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            MiddlewareHandle<TContext> handle = step(s);
            return new PipelineBehaviorHandle<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Terminate(MiddlewareFactory<MiddlewareAction<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            MiddlewareAction<TContext> handle = step(s);
            return new PipelineBehaviorTermination<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    public PipelineBuilder<TContext> Continue(MiddlewareFactory<MiddlewareAction<TContext>> step, ServiceLifetime? behaviorLifetime = null)
    {
        descriptors.Add(ServiceDescriptor.Describe(typeof(IPipelineBehavior<TContext>), (s) =>
        {
            MiddlewareAction<TContext> handle = step(s);
            return new PipelineBehaviorContinuation<TContext>(handle);
        }, behaviorLifetime ?? serviceLifetime));
        return this;
    }

    #endregion


    public IPipeline<TContext> Build(IServiceProvider serviceProvider)
    {
        return new Pipeline<TContext>(serviceProvider.GetServices<IPipelineBehavior<TContext>>());
    }

    public void Register(IServiceCollection serviceDescriptors)
    {
        foreach (var item in descriptors)
            serviceDescriptors.Add(item);
    }
}

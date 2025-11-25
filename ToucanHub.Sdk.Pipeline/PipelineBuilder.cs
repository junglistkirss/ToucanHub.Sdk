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

    private ServiceDescriptor FromFactory(Func<IServiceProvider, RichMiddlewareHandle<TContext>> func)
    {
        return ServiceDescriptor.Describe(typeof(RichMiddlewareHandle<TContext>), func, serviceLifetime);
    }

    private ServiceDescriptor Create<T>(Func<T, RichMiddlewareHandle<TContext>> handle)
        where T : class
    {
        return ServiceDescriptor.Describe(typeof(RichMiddlewareHandle<TContext>), (s) => handle(s.GetRequiredService<T>()), serviceLifetime);
    }
    
    public PipelineBuilder<TContext> Then(MiddlewareFactory<RichMiddlewareHandle<TContext>> step)
    {
        descriptors.Add(FromFactory((s) => step(s)));
        return this;
    }

    public PipelineBuilder<TContext> Then(RichMiddlewareHandle<TContext> handle)
    {
        descriptors.Add(FromFactory((s) => handle));
        return this;
    }
    
    public PipelineBuilder<TContext> Then<T>(Func<T, RichMiddlewareHandle<TContext>> handle)
        where T : class
    {
        descriptors.Add(Create(handle));
        return this;
    }
   
    public PipelineBuilder<TContext> Then<T>(Func<T, MiddlewareHandle<TContext>> handle)
       where T : class
    {
        descriptors.Add(Create<T>((src) => (ctx, next) => handle(src)(ctx, () => next(ctx))));
        return this;
    }

    public PipelineBuilder<TContext> Terminate<T>(Func<T, MiddlewareTermination<TContext>> handle)
        where T : class
    {
        descriptors.Add(Create<T>((src) => (ctx, _) => handle(src)(ctx)));
        return this;
    }

    public PipelineBuilder<TContext> Then(MiddlewareFactory<MiddlewareHandle<TContext>> step)
    {
        descriptors.Add(FromFactory((s) => (ctx, next) => step(s)(ctx, () => next(ctx))));
        return this;
    }

    public PipelineBuilder<TContext> Then(MiddlewareHandle<TContext> handle)
    {
        descriptors.Add(FromFactory((s) => (ctx, next) => handle(ctx, () => next(ctx))));
        return this;
    }

    public PipelineBuilder<TContext> Terminate(MiddlewareTermination<TContext> handle)
    {
        descriptors.Add(FromFactory((s) => new RichMiddlewareHandle<TContext>((ctx, next) => handle(ctx))));
        return this;
    }

    public IPipeline<TContext> Build(IServiceProvider serviceProvider)
    {
        return new SimplePipeline<TContext>(serviceProvider.GetServices<RichMiddlewareHandle<TContext>>());
    }

    public void Register(IServiceCollection serviceDescriptors)
    {

        foreach (var item in descriptors)
            serviceDescriptors.Add(item);
    }
}

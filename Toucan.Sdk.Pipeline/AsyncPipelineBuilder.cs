using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Sdk.Pipeline;

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

    private ServiceDescriptor FromFactory(Func<IServiceProvider, AsyncRichMiddlewareHandle<TContext>> func)
    {
        return ServiceDescriptor.Describe(typeof(AsyncRichMiddlewareHandle<TContext>), func, serviceLifetime);
    }

    private ServiceDescriptor Create<T>(Func<T, AsyncRichMiddlewareHandle<TContext>> handle)
        where T : class
    {
        return ServiceDescriptor.Describe(typeof(AsyncRichMiddlewareHandle<TContext>), (s) => handle(s.GetRequiredService<T>()), serviceLifetime);
    }

    // public AsyncPipelineBuilder<TContext> Then(AsyncRichMiddlewareHandle<TContext> handle)
    // {
    //     descriptors.Add(FromFactory((s) => handle));
    //     return this;
    // }
    public AsyncPipelineBuilder<TContext> Terminate(AsyncMiddlewareActionHandle<TContext> handle)
    {
        descriptors.Add(FromFactory((s) => new AsyncRichMiddlewareHandle<TContext>((ctx, next) => handle(ctx))));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Terminate(MiddlewareActionHandle<TContext> handle)
    {
        descriptors.Add(FromFactory((s) => new AsyncRichMiddlewareHandle<TContext>((ctx, next) =>
        {
            handle(ctx);
            return ValueTask.CompletedTask;
        })));
        return this;
    }
    public AsyncPipelineBuilder<TContext> Then(AsyncMiddlewareHandle<TContext> handle)
    {
        descriptors.Add(FromFactory(_ => new AsyncRichMiddlewareHandle<TContext>((ctx, next) => handle(ctx, () => next(ctx)))));
        return this;
    }
    public AsyncPipelineBuilder<TContext> Then(MiddlewareFactory<AsyncMiddlewareHandle<TContext>> step)
    {
        descriptors.Add(FromFactory((s) => new AsyncRichMiddlewareHandle<TContext>((ctx,next) => step(s)(ctx, () => next(ctx)))));
        return this;
    }
    public AsyncPipelineBuilder<TContext> Then(MiddlewareFactory<AsyncRichMiddlewareHandle<TContext>> step)
    {
        descriptors.Add(FromFactory((s) => step(s)));
        return this;
    }
    public AsyncPipelineBuilder<TContext> Then(AsyncRichMiddlewareHandle<TContext> handle)
    {
        descriptors.Add(FromFactory((s) => handle));
        return this;
    }
    public AsyncPipelineBuilder<TContext> Then<T>(Func<T, AsyncRichMiddlewareHandle<TContext>> handle)
       where T : class
    {
        descriptors.Add(Create<T>(handle));
        return this;
    }

    public AsyncPipelineBuilder<TContext> Then<T>(Func<T, AsyncMiddlewareHandle<TContext>> handle)
       where T : class
    {
        descriptors.Add(Create<T>(
            (src) => 
                new AsyncRichMiddlewareHandle<TContext>((ctx, next) => handle(src)(ctx, () => next(ctx)))
            ));
        return this;
    }

    
    public AsyncPipelineBuilder<TContext> Then<T>(Func<T, AsyncMiddlewareActionHandle<TContext>> handle)
       where T : class
    {
        descriptors.Add(Create<T>(
            (src) => 
                new AsyncRichMiddlewareHandle<TContext>((ctx, _) => handle(src)(ctx))
            ));
        return this;
    }

    public IAsyncPipeline<TContext> Build(IServiceProvider serviceProvider)
    {
        return new AsyncPipeline<TContext>(serviceProvider.GetServices<AsyncRichMiddlewareHandle<TContext>>());
    }

    public void Register(IServiceCollection serviceDescriptors)
    {
        foreach (ServiceDescriptor item in descriptors)
            serviceDescriptors.Add(item);
    }
}

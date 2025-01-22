using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ClearExtensions;

namespace Toucan.Sdk.Pipeline.Tests;

public class PipelineTest : IDisposable
{
    private static MiddlewareActionHandle<CounterContext> MiddlewareAction(string i) => ctx => ctx.Counter.Add(i);
    private static AsyncMiddlewareActionHandle<CounterContext> MiddlewareAsyncAction(string i) => ctx => { ctx.Counter.Add(i); return ValueTask.CompletedTask; };

    private static MiddlewareHandle<CounterContext> MiddlewareHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); next(); };
    private static AsyncMiddlewareHandle<CounterContext> MiddlewareAsyncHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); return next(); };

    private static RichMiddlewareHandle<CounterContext> RichMiddlewareHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); next(ctx); };
    private static AsyncRichMiddlewareHandle<CounterContext> RichMiddlewareAsyncHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); return next(ctx); };

    private static MiddlewareFactory<MiddlewareHandle<CounterContext>> MiddlewareFactory(string i) => _ => PassMiddleware.Handle(ctx => ctx.Counter.Add(i));
    private static MiddlewareFactory<AsyncMiddlewareHandle<CounterContext>> MiddlewareAsyncFactory(string i) => _ => PassMiddleware.HandleAsync(ctx => ctx.Counter.Add(i));


    private static MiddlewareFactory<RichMiddlewareHandle<CounterContext>> RichMiddlewareFactory(string i) => _ => PassMiddleware.RichHandle(ctx => ctx.Counter.Add(i));
    private static MiddlewareFactory<AsyncRichMiddlewareHandle<CounterContext>> RichMiddlewareAsyncFactory(string i) => _ => PassMiddleware.RichHandleAsync(ctx => ctx.Counter.Add(i));


    private readonly RichMiddlewareHandle<CounterContext> middlewareSimple;
    private readonly AsyncRichMiddlewareHandle<CounterContext> asyncMiddlewareSimple;
    public PipelineTest()
    {
        middlewareSimple = Substitute.For<RichMiddlewareHandle<CounterContext>>();
        asyncMiddlewareSimple = Substitute.For<AsyncRichMiddlewareHandle<CounterContext>>();

        asyncMiddlewareSimple(default!, default(RichNextAsyncDelegate<CounterContext>)!)
           .ReturnsForAnyArgs(ci => ci.ArgAt<RichNextAsyncDelegate<CounterContext>>(1).Invoke(ci.ArgAt<CounterContext>(0)));

        middlewareSimple
            .WhenForAnyArgs(x => x(default!, default(RichNextDelegate<CounterContext>)!))
            .Do(ci => ci.ArgAt<RichNextDelegate<CounterContext>>(1).Invoke(ci.ArgAt<CounterContext>(0)));
    }

    public void Dispose()
    {
        middlewareSimple.ClearSubstitute();
        asyncMiddlewareSimple.ClearSubstitute();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Build__TestSimple()
    {

        PipelineBuilder<CounterContext> builder = PipelineBuilder<CounterContext>
            .CreateBuilder()
            .Then(PassMiddleware.Handle(PassMiddleware.DefaultAction))
            .Then(RichMiddlewareFactory("RichFactory"))
            .Then(MiddlewareFactory("Factory"))
            .Then(MiddlewareHandle("Handle"))
            .Then(RichMiddlewareHandle("RichHandle"))
            .Then(middlewareSimple)
           .Terminate(MiddlewareAction("Action"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        {
            using IServiceScope scope = serviceProvider.CreateScope();
            CounterContext ctx = new();
            IPipeline<CounterContext> pipe = builder.Build(scope.ServiceProvider);
            pipe.Execute(ctx);
            Assert.Equal(["DefaultPass", "RichFactory", "Factory", "Handle", "RichHandle", "Action"], ctx.Counter);
            middlewareSimple.Received(1)(Arg.Any<CounterContext>(), Arg.Any<RichNextDelegate<CounterContext>>());
        }

        middlewareSimple.ClearReceivedCalls();

        {
            CounterContext ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IPipeline<CounterContext> pipe = scope.ServiceProvider.GetRequiredService<IPipeline<CounterContext>>();
            pipe.Execute(ctx);
            Assert.Equal(["DefaultPass", "RichFactory", "Factory", "Handle", "RichHandle", "Action"], ctx.Counter);
            middlewareSimple.Received(1)(Arg.Any<CounterContext>(), Arg.Any<RichNextDelegate<CounterContext>>());
        }
    }

    [Fact]
    public async Task Build__TestAsyncSimple__Completed()
    {
        AsyncPipelineBuilder<CounterContext> builder = AsyncPipelineBuilder<CounterContext>
            .CreateBuilder()
            .Then(PassMiddleware.HandleAsync(PassMiddleware.DefaultAction))
           .Then(RichMiddlewareAsyncFactory("RichAsyncFactory"))
           .Then(MiddlewareAsyncFactory("AsyncFactory"))
           //.Then(middlewareHandle)
           .Then(MiddlewareAsyncHandle("AsyncHandle"))
           .Then(RichMiddlewareAsyncHandle("RichAsyncHandle"))
           .Then(asyncMiddlewareSimple)
           .Terminate(MiddlewareAction("Action"))
           .Terminate(MiddlewareAsyncAction("AsyncAction"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        {
            CounterContext ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContext> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "Action"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContext>(), Arg.Any<RichNextAsyncDelegate<CounterContext>>());
        }

        asyncMiddlewareSimple.ClearReceivedCalls();
        {
            CounterContext ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContext> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContext>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "Action"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContext>(), Arg.Any<RichNextAsyncDelegate<CounterContext>>());
        }
    }

    [Fact]
    public async Task Build__TestAsyncSimple__CompletedAsync()
    {

        AsyncPipelineBuilder<CounterContext> builder = AsyncPipelineBuilder<CounterContext>
            .CreateBuilder()
            .Then(PassMiddleware.HandleAsync(PassMiddleware.DefaultAction))
           .Then(RichMiddlewareAsyncFactory("RichAsyncFactory"))
           .Then(MiddlewareAsyncFactory("AsyncFactory"))
           //.Then(middlewareHandle)
           .Then(MiddlewareAsyncHandle("AsyncHandle"))
           .Then(RichMiddlewareAsyncHandle("RichAsyncHandle"))
           .Then(asyncMiddlewareSimple)
           .Terminate(MiddlewareAsyncAction("AsyncAction"))
           .Terminate(MiddlewareAction("Action"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        {
            CounterContext ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContext> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "AsyncAction"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContext>(), Arg.Any<RichNextAsyncDelegate<CounterContext>>());
        }

        asyncMiddlewareSimple.ClearReceivedCalls();
        {
            CounterContext ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContext> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContext>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "AsyncAction"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContext>(), Arg.Any<RichNextAsyncDelegate<CounterContext>>());
        }
    }

    [Fact]
    public async Task Build__TestAsyncSimple__Throws()
    {
        AsyncPipelineBuilder<CounterContext> builder = AsyncPipelineBuilder<CounterContext>
            .CreateBuilder()
#pragma warning disable CS1998 // Cette m�thode async n'a pas d'op�rateur 'await' et elle s'ex�cutera de fa�on synchrone
           .Terminate(async (ctx) =>
           {
               throw new NotImplementedException();
           })
#pragma warning restore CS1998 // Cette m�thode async n'a pas d'op�rateur 'await' et elle s'ex�cutera de fa�on synchrone
           .Then((ctx, next) => next(ctx));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IAsyncPipeline<CounterContext> pipe = builder.Build(scope.ServiceProvider);
        await Assert.ThrowsAnyAsync<Exception>(async () => await pipe.ExecuteAsync(new CounterContext()));

    }
}

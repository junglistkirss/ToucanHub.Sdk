using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ClearExtensions;

namespace Toucan.Sdk.Pipeline.Tests;

public class PipelineOutTest : IDisposable
{
    private static MiddlewareActionHandle<CounterContextWithOutput> MiddlewareAction(string i, string result) => ctx =>
    {
        ctx.Counter.Add(i);
        ctx.Output = result;
    };
    private static AsyncMiddlewareActionHandle<CounterContextWithOutput> MiddlewareAsyncAction(string i, string result) => ctx =>
    {
        ctx.Counter.Add(i);
        ctx.Output = result;
        return ValueTask.CompletedTask;
    };

    private static MiddlewareHandle<CounterContextWithOutput> MiddlewareHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); next(); };
    private static AsyncMiddlewareHandle<CounterContextWithOutput> MiddlewareAsyncHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); return next(); };

    private static RichMiddlewareHandle<CounterContextWithOutput> RichMiddlewareHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); next(ctx); };
    private static AsyncRichMiddlewareHandle<CounterContextWithOutput> RichMiddlewareAsyncHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); return next(ctx); };

    private static MiddlewareFactory<MiddlewareHandle<CounterContextWithOutput>> MiddlewareFactory(string i) => _ => PassMiddlewareWithOutput.Handle(ctx => ctx.Counter.Add(i));
    private static MiddlewareFactory<AsyncMiddlewareHandle<CounterContextWithOutput>> MiddlewareAsyncFactory(string i) => _ => PassMiddlewareWithOutput.HandleAsync(ctx => ctx.Counter.Add(i));


    private static MiddlewareFactory<RichMiddlewareHandle<CounterContextWithOutput>> RichMiddlewareFactory(string i) => _ => PassMiddlewareWithOutput.RichHandle(ctx => ctx.Counter.Add(i));
    private static MiddlewareFactory<AsyncRichMiddlewareHandle<CounterContextWithOutput>> RichMiddlewareAsyncFactory(string i) => _ => PassMiddlewareWithOutput.RichHandleAsync(ctx => ctx.Counter.Add(i));


    private readonly RichMiddlewareHandle<CounterContextWithOutput> middlewareSimple;
    private readonly AsyncRichMiddlewareHandle<CounterContextWithOutput> asyncMiddlewareSimple;
    public PipelineOutTest()
    {
        middlewareSimple = Substitute.For<RichMiddlewareHandle<CounterContextWithOutput>>();
        asyncMiddlewareSimple = Substitute.For<AsyncRichMiddlewareHandle<CounterContextWithOutput>>();

        asyncMiddlewareSimple(default!, default(RichNextAsyncDelegate<CounterContextWithOutput>)!)
           .ReturnsForAnyArgs(ci => ci.ArgAt<RichNextAsyncDelegate<CounterContextWithOutput>>(1).Invoke(ci.ArgAt<CounterContextWithOutput>(0)));

        middlewareSimple.WhenForAnyArgs(x => x(default!, default(RichNextDelegate<CounterContextWithOutput>)!))
            .Do(ci => ci.ArgAt<RichNextDelegate<CounterContextWithOutput>>(1).Invoke(ci.ArgAt<CounterContextWithOutput>(0)));
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

        PipelineBuilder<CounterContextWithOutput> builder = PipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .Then(PassMiddlewareWithOutput.Handle(PassMiddlewareWithOutput.DefaultAction))
            .Then(RichMiddlewareFactory("RichFactory"))
            .Then(MiddlewareFactory("Factory"))
            .Then(MiddlewareHandle("Handle"))
            .Then(RichMiddlewareHandle("RichHandle"))
            .Then(middlewareSimple)
           .Terminate(MiddlewareAction("Action", "output"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        {
            using IServiceScope scope = serviceProvider.CreateScope();
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["DefaultPass", "RichFactory", "Factory", "Handle", "RichHandle", "Action"], ctx.Counter);
            middlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextDelegate<CounterContextWithOutput>>());
        }

        middlewareSimple.ClearReceivedCalls();

        {
            CounterContextWithOutput ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IPipeline<CounterContextWithOutput>>();
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["DefaultPass", "RichFactory", "Factory", "Handle", "RichHandle", "Action"], ctx.Counter);
            middlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextDelegate<CounterContextWithOutput>>());
        }
    }

    [Fact]
    public async Task Build__TestAsyncSimple__Completed()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .Then(PassMiddlewareWithOutput.HandleAsync(PassMiddlewareWithOutput.DefaultAction))
           .Then(RichMiddlewareAsyncFactory("RichAsyncFactory"))
           .Then(MiddlewareAsyncFactory("AsyncFactory"))
           //.Then(middlewareHandle)
           .Then(MiddlewareAsyncHandle("AsyncHandle"))
           .Then(RichMiddlewareAsyncHandle("RichAsyncHandle"))
           .Then(asyncMiddlewareSimple)
           .Terminate(MiddlewareAction("Action", "output1"))
           .Terminate(MiddlewareAsyncAction("AsyncAction", "output2"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        {
            CounterContextWithOutput ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output1", ctx.Output);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "Action"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>());
        }

        asyncMiddlewareSimple.ClearReceivedCalls();
        {
            CounterContextWithOutput ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContextWithOutput>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output1", ctx.Output);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "Action"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>());
        }
    }

    [Fact]
    public async Task Build__TestAsyncSimple__CompletedAsync()
    {

        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .Then(PassMiddlewareWithOutput.HandleAsync(PassMiddlewareWithOutput.DefaultAction))
           .Then(RichMiddlewareAsyncFactory("RichAsyncFactory"))
           .Then(MiddlewareAsyncFactory("AsyncFactory"))
           //.Then(middlewareHandle)
           .Then(MiddlewareAsyncHandle("AsyncHandle"))
           .Then(RichMiddlewareAsyncHandle("RichAsyncHandle"))
           .Then(asyncMiddlewareSimple)
           .Terminate(MiddlewareAsyncAction("AsyncAction", "output1"))
           .Terminate(MiddlewareAction("Action", "output2"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        {
            CounterContextWithOutput ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output1", ctx.Output);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "AsyncAction"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>());
        }

        asyncMiddlewareSimple.ClearReceivedCalls();
        {
            CounterContextWithOutput ctx = new();
            using IServiceScope scope = serviceProvider.CreateScope();
            IAsyncPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContextWithOutput>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output1", ctx.Output);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "AsyncAction"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>());
        }
    }

    [Fact]
    public async Task Build__TestAsyncSimple__Throws()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
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
        IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
        await Assert.ThrowsAnyAsync<Exception>(async () => await pipe.ExecuteAsync(new CounterContextWithOutput()));

    }

    [Fact]
    public async Task Build__TestAsyncSimple__NoReturn()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
           .Then((ctx, next) => next(ctx));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
        var ctx = new CounterContextWithOutput();
        await pipe.ExecuteAsync(ctx);
        Assert.Null(ctx.Output);
    }
}

using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ClearExtensions;
using System.Threading.Tasks;
using ToucanHub.Sdk.Pipeline.Exceptions;

namespace ToucanHub.Sdk.Pipeline.Tests;

internal sealed class Dependency
{
    public string Call()
    {
        return nameof(Dependency);
    }
    public ValueTask<string> CallAsync()
    {
        return ValueTask.FromResult(nameof(Dependency));
    }
}

public class PipelineOutTest : IDisposable
{
    private static MiddlewareAction<CounterContextWithOutput> MiddlewareAction(string i, string result) => ctx =>
    {
        ctx.Counter.Add(i);
        ctx.Output = result;
    };
    private static AsyncMiddlewareAction<CounterContextWithOutput> MiddlewareAsyncAction(string i, string result) => async ctx =>
    {
        ctx.Counter.Add(i);
        ctx.Output = result;
        await ValueTask.CompletedTask;
    };

    private static MiddlewareHandle<CounterContextWithOutput> MiddlewareHandle(string i) => (ctx, next) => { ctx.Counter.Add(i); next(); };
    private static AsyncMiddlewareHandle<CounterContextWithOutput> MiddlewareAsyncHandle(string i) => async (ctx, next) =>
    {
        ctx.Counter.Add(i);
        await next();
    };

    private static RichMiddlewareHandle<CounterContextWithOutput> RichMiddlewareHandle(string i) => (ctx, next) =>
    {
        ctx.Counter.Add(i);
        next(ctx);
    };
    private static AsyncRichMiddlewareHandle<CounterContextWithOutput> RichMiddlewareAsyncHandle(string i) => async (ctx, next) =>
    {
        ctx.Counter.Add(i);
        await next(ctx);
    };

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
        asyncMiddlewareSimple(default!, Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>())
           .ReturnsForAnyArgs(ci => ci.ArgAt<RichNextAsyncDelegate<CounterContextWithOutput>>(1).Invoke(ci.ArgAt<CounterContextWithOutput>(0)));
        middlewareSimple.WhenForAnyArgs(x => x(default!, Arg.Any<RichNextDelegate<CounterContextWithOutput>>()))
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

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["DefaultPass", "RichFactory", "Factory", "Handle", "RichHandle", "Action"], ctx.Counter);
            middlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextDelegate<CounterContextWithOutput>>());
        }

        middlewareSimple.ClearReceivedCalls();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IPipeline<CounterContextWithOutput>>();
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["DefaultPass", "RichFactory", "Factory", "Handle", "RichHandle", "Action"], ctx.Counter);
            middlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextDelegate<CounterContextWithOutput>>());
        }
    }

    [Fact]
    public async Task Build__TestDependencyAsync()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .ThenAsync<Dependency>(dep => async (ctx, next) =>
            {
                string result = await dep.CallAsync();
                ctx.Counter.Add(result);
                await next();
            })
           .Terminate(MiddlewareAction("Action", "output"));
        IServiceCollection descriptors = new ServiceCollection()
            .UsePipelines()
            .AddSingleton<Dependency>();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }

        middlewareSimple.ClearReceivedCalls();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContextWithOutput>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }
    }

    [Fact]
    public async Task Build__RichTestDependencyAsync()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .ThenAsync<Dependency>(dep => async (ctx, next) =>
            {
                string result = await dep.CallAsync();
                ctx.Counter.Add(result);
                await next(ctx);
            })
           .Terminate(MiddlewareAction("Action", "output"));
        IServiceCollection descriptors = new ServiceCollection()
            .UsePipelines()
            .AddSingleton<Dependency>();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }

        middlewareSimple.ClearReceivedCalls();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContextWithOutput>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }
    }

    [Fact]
    public async Task Build__TestDependencyTerminateAsync()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .TerminateAsync<Dependency>(dep => async (ctx) =>
            {
                ctx.Output = await dep.CallAsync();
            })
           .Terminate(MiddlewareAction("Action", "output"));
        IServiceCollection descriptors = new ServiceCollection()
            .UsePipelines()
            .AddSingleton<Dependency>();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("Dependency", ctx.Output);
        }

        middlewareSimple.ClearReceivedCalls();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContextWithOutput>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("Dependency", ctx.Output);
        }
    }

    [Fact]
    public void Build__TestDependency()
    {
        PipelineBuilder<CounterContextWithOutput> builder = PipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .Then<Dependency>(dep => (ctx, next) =>
            {
                ctx.Counter.Add(dep.Call());
                next();
            })
           .Terminate(MiddlewareAction("Action", "output"));
        IServiceCollection descriptors = new ServiceCollection()
            .UsePipelines()
            .AddSingleton<Dependency>();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }

        middlewareSimple.ClearReceivedCalls();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IPipeline<CounterContextWithOutput>>();
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }
    }

    [Fact]
    public void Build__RichTestDependency()
    {
        PipelineBuilder<CounterContextWithOutput> builder = PipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .Then<Dependency>(dep => (ctx, next) =>
            {
                ctx.Counter.Add(dep.Call());
                next(ctx);
            })
           .Terminate(MiddlewareAction("Action", "output"));
        IServiceCollection descriptors = new ServiceCollection()
            .UsePipelines()
            .AddSingleton<Dependency>();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }

        middlewareSimple.ClearReceivedCalls();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IPipeline<CounterContextWithOutput>>();
            pipe.Execute(ctx);
            Assert.Equal("output", ctx.Output);
            Assert.Equal(["Dependency", "Action"], ctx.Counter);
        }
    }

    [Fact]
    public void Build__TestDependencyTerminate()
    {
        PipelineBuilder<CounterContextWithOutput> builder = PipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .Terminate<Dependency>(dep => (ctx) =>
            {
                ctx.Output = dep.Call();
            })
           .Terminate(MiddlewareAction("Action", "output"));
        IServiceCollection descriptors = new ServiceCollection()
            .UsePipelines()
            .AddSingleton<Dependency>();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            pipe.Execute(ctx);
            Assert.Equal("Dependency", ctx.Output);
        }

        middlewareSimple.ClearReceivedCalls();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IPipeline<CounterContextWithOutput>>();
            pipe.Execute(ctx);
            Assert.Equal("Dependency", ctx.Output);
        }
    }

    [Fact]
    public async Task Build__TestAsyncSimple__Completed()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .ThenAsync(PassMiddlewareWithOutput.HandleAsync(PassMiddlewareWithOutput.DefaultAction))
           .ThenAsync(RichMiddlewareAsyncFactory("RichAsyncFactory"))
           .ThenAsync(MiddlewareAsyncFactory("AsyncFactory"))
           //.Then(middlewareHandle)
           .ThenAsync(MiddlewareAsyncHandle("AsyncHandle"))
           .ThenAsync(RichMiddlewareAsyncHandle("RichAsyncHandle"))
           .ThenAsync(asyncMiddlewareSimple)
           .Terminate(MiddlewareAction("Action", "output1"))
           .TerminateAsync(MiddlewareAsyncAction("AsyncAction", "output2"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output1", ctx.Output);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "Action"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>());
        }

        asyncMiddlewareSimple.ClearReceivedCalls();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
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
            .ThenAsync(PassMiddlewareWithOutput.HandleAsync(PassMiddlewareWithOutput.DefaultAction))
           .ThenAsync(RichMiddlewareAsyncFactory("RichAsyncFactory"))
           .ThenAsync(MiddlewareAsyncFactory("AsyncFactory"))
           //.Then(middlewareHandle)
           .ThenAsync(MiddlewareAsyncHandle("AsyncHandle"))
           .ThenAsync(RichMiddlewareAsyncHandle("RichAsyncHandle"))
           .ThenAsync(asyncMiddlewareSimple)
           .TerminateAsync(MiddlewareAsyncAction("AsyncAction", "output1"))
           .Terminate(MiddlewareAction("Action", "output2"));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output1", ctx.Output);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "AsyncAction"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>());
        }

        asyncMiddlewareSimple.ClearReceivedCalls();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CounterContextWithOutput ctx = new();
            IAsyncPipeline<CounterContextWithOutput> pipe = scope.ServiceProvider.GetRequiredService<IAsyncPipeline<CounterContextWithOutput>>();
            await pipe.ExecuteAsync(ctx);
            Assert.Equal("output1", ctx.Output);
            Assert.Equal(["DefaultPass", "RichAsyncFactory", "AsyncFactory", "AsyncHandle", "RichAsyncHandle", "AsyncAction"], ctx.Counter);
            await asyncMiddlewareSimple.Received(1)(Arg.Any<CounterContextWithOutput>(), Arg.Any<RichNextAsyncDelegate<CounterContextWithOutput>>());
        }
    }

    [Fact]
    public void Build__TestSimple__Throws()
    {
        PipelineBuilder<CounterContextWithOutput> builder = PipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
           .Terminate(new MiddlewareAction<CounterContextWithOutput>((ctx) =>
           {
               throw new NotImplementedException();
           }))
           .Then((ctx, next) => next(ctx));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
        FlowException ex = Assert.Throws<FlowException>(() => pipe.Execute(new CounterContextWithOutput()));
        Assert.IsType<NotImplementedException>(ex.InnerException);
    }

    [Fact]
    public async Task Build__TestAsyncSimple__Throws()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
           .TerminateAsync(new AsyncMiddlewareAction<CounterContextWithOutput>((ctx) =>
           {
               throw new NotImplementedException();
           }))
           .ThenAsync((ctx, next) => next(ctx));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
        FlowException ex = await Assert.ThrowsAsync<FlowException>(async () => await pipe.ExecuteAsync(new CounterContextWithOutput()));
        Assert.IsType<NotImplementedException>(ex.InnerException);
    }

    [Fact]
    public void Build__TestSimple__ThrowsDoubleCall()
    {
        PipelineBuilder<CounterContextWithOutput> builder = PipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .Then((ctx, next) =>
            {
                next();
                next();
            });
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
        FlowException ex = Assert.Throws<FlowException>(() => pipe.Execute(new CounterContextWithOutput()));
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public async Task Build__TestAsyncSimple__ThrowsDoubleCall()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
            .ThenAsync(async (ctx, next) =>
            {
                await next();
                await next();
            });
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
        FlowException ex = await Assert.ThrowsAsync<FlowException>(async () => await pipe.ExecuteAsync(new CounterContextWithOutput()));
        Assert.Null(ex.InnerException);
    }
    [Fact]
    public async Task Build__TestAsyncSimple__NoReturn()
    {
        AsyncPipelineBuilder<CounterContextWithOutput> builder = AsyncPipelineBuilder<CounterContextWithOutput>
            .CreateBuilder()
           .ThenAsync((ctx, next) => next(ctx));
        IServiceCollection descriptors = new ServiceCollection().UsePipelines();
        builder.Register(descriptors);
        IServiceProvider serviceProvider = descriptors.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IAsyncPipeline<CounterContextWithOutput> pipe = builder.Build(scope.ServiceProvider);
        CounterContextWithOutput ctx = new();
        await pipe.ExecuteAsync(ctx);
        Assert.Null(ctx.Output);
    }
}

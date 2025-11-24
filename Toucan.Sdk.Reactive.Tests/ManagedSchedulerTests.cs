using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System.Reactive.Linq;

namespace Toucan.Sdk.Reactive.Tests;

public class ManagedSchedulerTests
{
    private readonly TestScheduler schedule = new();

    [Fact]
    public async Task TestManagedSchedulerProvider()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        string? value = null!;
        IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();
        Guid srv = launcher.Initialize();
        IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
        sub.Register<string>(srv, o => o.ObserveOn(schedule).Subscribe(x =>
        {
            value = x;
        }));
        IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
        pub.Publish(srv, "Hello World");
        Assert.Null(value);
        schedule.AdvanceBy(1);
        Assert.Equal("Hello World", value);
    }

    [Fact]
    public async Task TestManagedSchedulerProvider2()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        string? value = null!;
        IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();
        Guid srv = launcher.Initialize();
        IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
        sub.Register<string>(srv, x => x.ObserveOn(schedule).Subscribe(x =>
        {
            value = x;
        }, () =>
        {
            value = "Goodbye";
        }));
        IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
        pub.Publish(srv, "Hello World");
        pub.Publish(srv, "Hello World 2");
        pub.Complete(srv);
        Assert.Null(value);
        schedule.AdvanceBy(1);
        Assert.Equal("Hello World", value);
        schedule.AdvanceBy(1);
        Assert.Equal("Hello World 2", value);
        schedule.AdvanceBy(1);
        Assert.Equal("Goodbye", value);
    }

    [Fact]
    public async Task TestManagedSchedulerProviderDefineService()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        string? value = null!;
        IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();
        Guid srv = Guid.NewGuid();
        launcher.Initialize(srv);
        IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
        sub.Register<string>(srv, x => x.ObserveOn(schedule).Subscribe(x =>
        {
            value = x;
        }, () =>
        {
            value = "Goodbye";
        }));
        IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
        pub.Publish(srv, "Hello World");
        pub.Publish(srv, "Hello World 2");
        Assert.Null(value);
        schedule.AdvanceBy(1);
        Assert.Equal("Hello World", value);
        launcher.Kill(srv);
        schedule.Start();

        Assert.NotEqual("Hello World 2", value);
        Assert.NotEqual("Goodbye", value);
    }

    [Fact]
    public async Task TestManagedSchedulerProviderNoChanges()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        string? value = null!;
        IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();

        IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
        Guid srv = Guid.NewGuid();
        sub.Register<string>(srv, x => x.ObserveOn(schedule).Subscribe(x =>
        {
            value = x;
        }, () =>
        {
            value = "Goodbye";
        }));
        IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
        pub.Publish(srv, "Hello World");
        pub.Publish(srv, "Hello World 2");
        pub.Complete(srv);
        schedule.Start();
        Assert.Null(value);
    }

    [Fact]
    public async Task TestManagedSchedulerProviderExceptionHandler()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        string? value = null!;
        IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();

        IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
        Guid srv = launcher.Initialize();
        sub.Register<string>(srv, x => x.ObserveOn(schedule).Subscribe(x =>
        {
            ArgumentException.ThrowIfNullOrEmpty(x);
            value = x;
        }, ex =>
        {
            value = ex.Message;
        }));
        IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
        pub.Publish(srv, "Hello World");
        Assert.Null(value);
        schedule.AdvanceBy(1);
        Assert.Equal("Hello World", value);
        pub.Publish(srv, string.Empty);
        Assert.Throws<ArgumentException>(() => schedule.AdvanceBy(1));
    }

    [Fact]
    public async Task TestManagedSchedulerProviderExceptionHandlerThrow()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        string? value = null!;
        IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();

        IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
        Guid srv = launcher.Initialize();
        sub.Register<string>(srv, x => x.ObserveOn(schedule).Subscribe(x =>
        {
            value = x;
        }, ex =>
        {
            value = ex.Message;
        }));
        IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
        pub.Publish(srv, "Hello World");
        Assert.Null(value);
        schedule.AdvanceBy(1);
        Assert.Equal("Hello World", value);
        pub.Throw(srv, new Exception("bad vibes"));
        schedule.Start();
        Assert.Equal("bad vibes", value);
    }
}

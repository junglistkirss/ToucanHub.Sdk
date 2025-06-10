using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System.Reactive.Subjects;

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
        services.TryAddSingleton<IReactiveLauncherSchedulerProvider>(new TestSubscriptionSchedulerProvider(schedule));
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using (AsyncServiceScope scope = serviceProvider.CreateAsyncScope())
        {
            string? value = null!;
            IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();
            Guid srv = launcher.Initialize();
            IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
            sub.Subscribe<string>(srv, x =>
            {
                value = x;
            });
            IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
            pub.Publish(srv, "Hello World");
            Assert.Null(value);
            schedule.AdvanceBy(1);
            Assert.Equal("Hello World", value);
        }
    }

    [Fact]
    public async Task TestManagedSchedulerProvider2()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.TryAddSingleton<IReactiveLauncherSchedulerProvider>(new TestSubscriptionSchedulerProvider(schedule));
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using (AsyncServiceScope scope = serviceProvider.CreateAsyncScope())
        {
            string? value = null!;
            IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();
            Guid srv = launcher.Initialize();
            IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
            sub.Subscribe<string>(srv, x =>
            {
                value = x;
            }, null, () =>
            {
                value = "Goodbye";
            });
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
    }

    [Fact]
    public async Task TestManagedSchedulerProviderDefineService()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.TryAddSingleton<IReactiveLauncherSchedulerProvider>(new TestSubscriptionSchedulerProvider(schedule));
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using (AsyncServiceScope scope = serviceProvider.CreateAsyncScope())
        {
            string? value = null!;
            IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();
            Guid srv = Guid.NewGuid();
            launcher.Initialize(srv);
            IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
            sub.Subscribe<string>(srv, x =>
            {
                value = x;
            }, null, () =>
            {
                value = "Goodbye";
            });
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
    }

    [Fact]
    public async Task TestManagedSchedulerProviderNoChanges()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.TryAddSingleton<IReactiveLauncherSchedulerProvider>(new TestSubscriptionSchedulerProvider(schedule));
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using (AsyncServiceScope scope = serviceProvider.CreateAsyncScope())
        {
            string? value = null!;
            IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();
            
            IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
            Guid srv = Guid.NewGuid();
            sub.Subscribe<string>(srv, x =>
            {
                value = x;
            }, null, () =>
            {
                value = "Goodbye";
            });
            IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
            pub.Publish(srv, "Hello World");
            pub.Publish(srv, "Hello World 2");
            pub.Complete(srv);
            schedule.Start();
            Assert.Null(value);
        }
    }

    [Fact]
    public async Task TestManagedSchedulerProviderExceptionHandler()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.TryAddSingleton<IReactiveLauncherSchedulerProvider>(new TestSubscriptionSchedulerProvider(schedule));
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using (AsyncServiceScope scope = serviceProvider.CreateAsyncScope())
        {
            string? value = null!;
            IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();

            IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
            Guid srv = launcher.Initialize();
            sub.Subscribe<string>(srv, x =>
            {
                ArgumentException.ThrowIfNullOrEmpty(x);
                value = x;
            }, ex =>
            {
                value = ex.Message;
            });
            IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
            pub.Publish(srv, "Hello World");
            pub.Publish<string>(srv, string.Empty);
            Assert.Null(value);
            schedule.AdvanceBy(1);
            Assert.Equal("Hello World", value);
            schedule.Start();
            Assert.NotEqual("Hello World", value);
        }
    }

    [Fact]
    public async Task TestManagedSchedulerProviderExceptionHandlerThrow()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.TryAddSingleton<IReactiveLauncherSchedulerProvider>(new TestSubscriptionSchedulerProvider(schedule));
        services.AddManagedReactiveHostedService(Guid.NewGuid);
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        await using (AsyncServiceScope scope = serviceProvider.CreateAsyncScope())
        {
            string? value = null!;
            IReactiveLauncher<Guid> launcher = scope.ServiceProvider.GetRequiredService<IReactiveLauncher<Guid>>();

            IReactiveManagedSubscriber<Guid> sub = scope.ServiceProvider.GetRequiredService<IReactiveManagedSubscriber<Guid>>();
            Guid srv = launcher.Initialize();
            sub.Subscribe<string>(srv, x =>
            {
                value = x;
            }, ex =>
            {
                value = ex.Message;
            });
            IReactiveManagedDispatcher<Guid> pub = scope.ServiceProvider.GetRequiredService<IReactiveManagedDispatcher<Guid>>();
            pub.Publish(srv, "Hello World");
            pub.Throw(srv, new Exception("bad vibes"));
            Assert.Null(value);
            schedule.AdvanceBy(1);
            Assert.Equal("Hello World", value);
            schedule.Start();
            Assert.Equal("bad vibes", value);
        }
    }
}

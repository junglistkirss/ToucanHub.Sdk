using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Reactive.Concurrency;

namespace ToucanHub.Sdk.Reactive.Tests;
public class SchedulerTests
{
    [Fact]
    public async Task NoSchedulerProvider()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.AddReactiveHostedService();
        var serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        ISubscriptionManager manager = serviceProvider.GetRequiredService<ISubscriptionManager>();
        bool started = await manager.WaitForStart(CancellationToken.None);
        Assert.True(started);
        foreach (var item in hosteds)
        {
            await item.StopAsync(CancellationToken.None);
        }
    }

    private class TestSchedulerProvider : ISubscrptionsSchedulerProvider
    {
        private readonly EventLoopScheduler scheduler = new();
        public IScheduler Scheduler => scheduler;
    }

    [Fact]
    public async Task WithSchedulerProvider()
    {
        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.TryAddSingleton<ISubscrptionsSchedulerProvider>(new TestSchedulerProvider());
        services.AddReactiveHostedService();
        var serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        ISubscriptionManager manager = serviceProvider.GetRequiredService<ISubscriptionManager>();
        bool started = await manager.WaitForStart(CancellationToken.None);
        Assert.True(started);
        foreach (var item in hosteds)
        {
            await item.StopAsync(CancellationToken.None);
        }
    }


    [Fact]
    public async Task WithMockSchedulerProvider()
    {


        IServiceCollection services = new ServiceCollection();
        services.TryAddSingleton(s => Substitute.For<ILoggerFactory>());
        services.TryAddScoped(typeof(ILogger<>), typeof(MockLogger<>));
        services.TryAddScoped(s => Substitute.For<ILogger>());
        services.TryAddSingleton(s => Substitute.For<ISubscrptionsSchedulerProvider>());
        services.AddReactiveHostedService();
        var serviceProvider = services.BuildServiceProvider();
        IEnumerable<IHostedService> hosteds = serviceProvider.GetServices<IHostedService>();
        foreach (var item in hosteds)
        {
            await item.StartAsync(CancellationToken.None);
        }
        ISubscriptionManager manager = serviceProvider.GetRequiredService<ISubscriptionManager>();
        bool started = await manager.WaitForStart(CancellationToken.None);
        Assert.True(started);
        foreach (var item in hosteds)
        {
            await item.StopAsync(CancellationToken.None);
        }
    }
}

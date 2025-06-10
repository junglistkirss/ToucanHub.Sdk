using System.Reactive.Concurrency;

namespace Toucan.Sdk.Reactive.Tests;

internal class TestSubscriptionSchedulerProvider(IScheduler scheduler) : IReactiveLauncherSchedulerProvider
{
    public IScheduler GetScheduler() => scheduler;
}

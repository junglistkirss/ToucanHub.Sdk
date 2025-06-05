using System.Reactive.Concurrency;

namespace Toucan.Sdk.Reactive;

public interface IReactiveLauncherSchedulerProvider
{
    IScheduler GetScheduler();
}

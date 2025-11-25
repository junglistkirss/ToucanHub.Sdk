namespace ToucanHub.Sdk.Reactive;

public interface IReactiveLauncher<TServiceId>
    where TServiceId : struct
{
    Task<bool> WaitForStart(CancellationToken cancellationToken = default);

    TServiceId Initialize();
    void Initialize(TServiceId serviceId);
    void Kill(TServiceId serviceId);
    IObservable<ChildServiceInfo<TServiceId>> Observe();
}

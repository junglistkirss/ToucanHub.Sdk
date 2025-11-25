namespace ToucanHub.Sdk.Reactive;

public record class ChildServiceInfo<TServiceId>(TServiceId Id, ManagedServiceState State)
    where TServiceId : struct;

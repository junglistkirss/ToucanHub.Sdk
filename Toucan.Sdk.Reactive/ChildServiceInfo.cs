namespace Toucan.Sdk.Reactive;

public record class ChildServiceInfo<TServiceId>(TServiceId Id, ManagedServiceState State)
    where TServiceId : IEquatable<TServiceId>;

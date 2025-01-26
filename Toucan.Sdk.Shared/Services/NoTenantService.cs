namespace Toucan.Sdk.Shared.Services;

public sealed class NoTenantService : ITenantService
{
    //TODO : tenant resolution
    public Tenant GetTenant() => Tenant.Unspecified;
}

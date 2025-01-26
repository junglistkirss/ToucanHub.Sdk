using System.Collections.Immutable;
using Toucan.Sdk.Contracts.Messages;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Application.Context;

public static class ContextMetadataKeys
{
    public const string CultureKey = "culture";
}

public interface IContext
{
    Guid Id { get; }
    CancellationToken CancellationToken { get; }
    Tenant Origin { get; }
    ImmutableHashSet<Metadata> Metadatas { get; }
    IUserContext User { get; }
}

public sealed class NoopUserContext : IUserContext
{
    public static readonly IUserContext Empty = new NoopUserContext();
    private NoopUserContext() { }

    public RefToken Issuer => throw new NotImplementedException();

    public Role[] Roles => throw new NotImplementedException();

    public ValueTask<PermissionSet> GetAllPermissions() => throw new NotImplementedException();

    public ValueTask<AppScope[]> GetAllScopes()
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> HasPermission(Permission permission)
    {
        throw new NotImplementedException();
    }

    public bool HasScope(AppScope scope)
    {
        throw new NotImplementedException();
    }

    ValueTask<bool> IUserContext.HasScope(AppScope scope)
    {
        throw new NotImplementedException();
    }
}

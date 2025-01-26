using System.Collections.Immutable;
using Toucan.Sdk.Application.Context;
using Toucan.Sdk.Contracts.Messages;

namespace Toucan.Sdk.Api.Features;

internal sealed class ApiContext : IContext
{
    public static readonly IContext Empty = new ApiContext(Tenant.Unspecified, NoopUserContext.Empty, CancellationToken.None);

    public static IContext Create(Tenant tenant, IUserContext userContext, CancellationToken cancellationToken)
        => new ApiContext(tenant, userContext, cancellationToken);
    private ApiContext(Tenant tenant, IUserContext userContext, CancellationToken cancellationToken)
    {
        Origin = tenant;
        User = userContext;
        CancellationToken = cancellationToken;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public CancellationToken CancellationToken { get; }
    public Tenant Origin { get; }
    public IUserContext User { get; }
    public ImmutableHashSet<Metadata> Metadatas { get; set; } = [];
}

namespace Toucan.Sdk.Shared.Services;


public interface IAliasService
{
    ValueTask Insert(DomainId contextId, DomainId key, string aliasTypeName, Slug aliasName, CancellationToken cancellationToken);
    ValueTask Upsert(DomainId contextId, DomainId key, string aliasTypeName, Slug aliasName, CancellationToken cancellationToken);
    ValueTask Delete(DomainId contextId, DomainId key, string aliasTypeName, CancellationToken cancellationToken);
    ValueTask<DomainId> GetId(Slug key, string aliasTypeName, CancellationToken cancellationToken);
    ValueTask<DomainId> GetId(DomainId contextId, Slug key, string aliasTypeName, CancellationToken cancellationToken);
}
public interface IAliasResolverService
{
    ValueTask<NamedId> GetAlias(DomainId id, string aliasTypeName, CancellationToken cancellationToken);
    ValueTask<NamedId[]> GetAliases(DomainId[] ids, string aliasTypeName, CancellationToken cancellationToken);

    ValueTask<NamedId> GetAlias(DomainId contextId, DomainId id, string aliasTypeName, CancellationToken cancellationToken);
    ValueTask<NamedId[]> GetAliases(DomainId contextId, DomainId[] ids, string aliasTypeName, CancellationToken cancellationToken);

}
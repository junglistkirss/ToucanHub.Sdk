using System.Diagnostics;

namespace Toucan.Sdk.Contracts.Names;

[DebuggerDisplay("{Id,nq} = {Name,nq}")]

public readonly record struct NamedId(DomainId Id, Slug Name)
{
    public static readonly NamedId Empty = new NamedId(DomainId.Empty, Slug.Empty);


    public static implicit operator DomainId(NamedId id) => id.Id;
}


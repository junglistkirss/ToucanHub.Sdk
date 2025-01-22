using System.Collections.Immutable;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Messages;


public interface ISignal
{
    DateTimeOffset Timestamp { get; }

    RefToken Issuer { get; }

    Tenant Origin { get; }

    ImmutableHashSet<Metadata> Metadatas { get; }
}

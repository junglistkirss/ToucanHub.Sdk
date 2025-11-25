using System.Collections.Immutable;
using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Messages;


public interface ISignal
{
    DateTimeOffset Timestamp { get; }

    ActorReference Issuer { get; }

    Tenant Origin { get; }

    ImmutableHashSet<Metadata> Metadatas { get; }
}

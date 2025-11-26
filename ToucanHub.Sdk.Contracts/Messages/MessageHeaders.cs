using System.Collections.Immutable;
using System.Globalization;
using ToucanHub.Sdk.Contracts.Names;

namespace ToucanHub.Sdk.Contracts.Messages;

public readonly record struct MessageHeaders : ISignal
{
    public const string TimestampKey = "timestamp";
    public const string OriginKey = "origin";
    public const string IssuerKey = "issuer";

    public static MessageHeaders Create(DateTimeOffset timestamp, ActorReference issuer, Tenant origin, params Metadata[] metadatas) => new()
    {
        Metadatas = [
           new Metadata{ Key = IssuerKey, Value = issuer },
           new Metadata{ Key = TimestampKey, Value = timestamp.ToString("o", CultureInfo.InvariantCulture) },
           new Metadata{ Key = OriginKey, Value = origin },
            ..metadatas ?? [],
        ],
    };

    public static readonly MessageHeaders Empty = new();

    private ActorReference ResolveIssuer()
    {
        Metadata meta = Metadatas.FirstOrDefault(x => x.Key == IssuerKey);
        if (ActorReference.TryParse(meta.Value, out ActorReference typedMeta))
            return typedMeta;
        return ActorReference.Anonymous;
    }

    private DateTimeOffset ResolveTimestamp()
    {
        Metadata meta = Metadatas.FirstOrDefault(x => x.Key == TimestampKey);
        if (DateTimeOffset.TryParse(meta.Value, out DateTimeOffset typedMeta))
            return typedMeta;
        return DateTimeOffset.UtcNow;
    }
    private Tenant ResolveOrigin()
    {
        Metadata meta = Metadatas.FirstOrDefault(x => x.Key == OriginKey);
        if (Tenant.TryParse(meta.Value, out Tenant typedMeta))
            return typedMeta;
        return Tenant.Unspecified;
    }

    public MessageHeaders()
    {}
    public ImmutableHashSet<Metadata> Metadatas { get; init; } = [];

    public DateTimeOffset Timestamp => ResolveTimestamp();
    public ActorReference Issuer => ResolveIssuer();
    public Tenant Origin => ResolveOrigin();
}

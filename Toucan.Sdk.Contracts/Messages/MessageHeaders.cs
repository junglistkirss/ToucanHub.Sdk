using System.Collections.Immutable;
using System.Globalization;
using Toucan.Sdk.Contracts.Names;

namespace Toucan.Sdk.Contracts.Messages;

public readonly record struct MessageHeaders : ISignal
{
    public const string TimestampKey = "timestamp";
    public const string OriginKey = "origin";
    public const string IssuerKey = "issuer";

    public static MessageHeaders Create(DateTimeOffset timestamp, RefToken issuer, Tenant origin, params Metadata[] metadatas) => new()
    {
        Metadatas = [
           new Metadata{ Key = IssuerKey, Value = issuer },
           new Metadata{ Key = TimestampKey, Value = timestamp.ToString("o", CultureInfo.InvariantCulture) },
           new Metadata{ Key = OriginKey, Value = origin },
            ..metadatas ?? [],
        ],
    };

    public static readonly MessageHeaders Empty = new();

    private RefToken ResolveIssuer()
    {
        Metadata meta = Metadatas.FirstOrDefault(x => x.Key == IssuerKey);
        if (RefToken.TryParse(meta.Value, out RefToken typedMeta))
            return typedMeta;
        return RefToken.Anonymous;
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
    {
        //    timestamp = new Lazy<DateTimeOffset>(ResolveTimestamp, true);
        //    issuer = new Lazy<RefToken>(ResolveIssuer, true);
        //    origin = new Lazy<Tenant>(ResolveOrigin, true);
    }
    public ImmutableHashSet<Metadata> Metadatas { get; init; } = [];

    public DateTimeOffset Timestamp => ResolveTimestamp();//{ get; [Obsolete("Use Metadatas collections")] init; }
    public RefToken Issuer => ResolveIssuer();//{ get; [Obsolete("Use Metadatas collections")] init; }
    public Tenant Origin => ResolveOrigin();//{ get; [Obsolete("Use Metadatas collections")] init; }
}

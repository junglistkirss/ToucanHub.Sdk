using System.Collections.ObjectModel;

namespace Toucan.Sdk.Contracts.Security;

public sealed class RealmSet : ReadOnlyCollection<Realm>
{
    public static new readonly RealmSet Empty = new(Array.Empty<string>());

    private readonly Lazy<string> display;

    public RealmSet(params Realm[] realm)
        : this(realm?.Distinct()!)
    {
    }

    public RealmSet(params string[] realm)
        : this(realm?.Select(x => new Realm(x)).Distinct()!)
    {
    }

    public RealmSet(IEnumerable<string> realm)
        : this(realm?.Select(x => new Realm(x)).Distinct()!)
    {
    }

    public RealmSet(IEnumerable<Realm> realm)
        : this(realm?.Distinct()?.ToList()!)
    {
    }

    public RealmSet(IList<Realm> realm)
        : base(realm ?? [])
    {
        display = new Lazy<string>(() => string.Join(";", this));
    }

    public RealmSet Add(string realm)
    {
        if (string.IsNullOrWhiteSpace(realm))
            throw new ArgumentException($"'{nameof(realm)}' ne peut pas avoir une valeur null ou être un espace blanc.", nameof(realm));

        return Add(new Realm(realm));
    }

    public RealmSet Add(Realm realm)
    {
        return new RealmSet(this.Union([realm]).Distinct());
    }

    public bool Allows(Realm other) => this.Any(x => x.Equals(other));

    public override string ToString() => display.Value;

    //public IEnumerable<string> ToIds() => this.Select(x => x.Id);
}

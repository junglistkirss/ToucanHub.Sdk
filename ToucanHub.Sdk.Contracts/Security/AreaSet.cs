using System.Collections.ObjectModel;

namespace ToucanHub.Sdk.Contracts.Security;

public sealed class AreaSet : ReadOnlyCollection<Area>
{
    public static new readonly AreaSet Empty = new(Array.Empty<string>());

    private readonly Lazy<string> display;

    public AreaSet(params Area[] area)
        : this(area?.Distinct()!)
    {
    }

    public AreaSet(params string[] area)
        : this(area?.Select(x => new Area(x)).Distinct()!)
    {
    }

    public AreaSet(IEnumerable<string> area)
        : this(area?.Select(x => new Area(x)).Distinct()!)
    {
    }

    public AreaSet(IEnumerable<Area> area)
        : this(area?.Distinct()?.ToList()!)
    {
    }

    public AreaSet(IList<Area> area)
        : base(area ?? [])
    {
        display = new Lazy<string>(() => string.Join(";", this));
    }

    public AreaSet Add(string area)
    {
        if (string.IsNullOrWhiteSpace(area))
            throw new ArgumentException($"'{nameof(area)}' ne peut pas avoir une valeur null ou être un espace blanc.", nameof(area));

        return Add(new Area(area));
    }

    public AreaSet Add(Area area)
    {
        return new AreaSet(this.Union([area]).Distinct());
    }

    public bool Allows(Area other) => this.Any(x => x.Equals(other));

    public override string ToString() => display.Value;

    //public IEnumerable<string> ToIds() => this.Select(x => x.Id);
}

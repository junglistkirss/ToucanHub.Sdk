using System.Collections.ObjectModel;

namespace ToucanHub.Sdk.Contracts.Security;

public sealed class PermissionSet : ReadOnlyCollection<Permission>
{
    public static new readonly PermissionSet Empty = new(Array.Empty<string>());

    private readonly Lazy<string> display;

    public PermissionSet(params Permission[] permissions)
        : this(permissions?.ToList()!)
    {
    }

    public PermissionSet(params string[] permissions)
        : this(permissions?.Select(x => new Permission(x)).ToList()!)
    {
    }

    public PermissionSet(IEnumerable<string> permissions)
        : this(permissions?.Select(x => new Permission(x)).ToList()!)
    {
    }

    public PermissionSet(IEnumerable<Permission> permissions)
        : this(permissions?.ToList()!)
    {
    }

    public PermissionSet(IList<Permission> permissions)
        : base(permissions)
    {
        display = new Lazy<string>(() => string.Join(";", this));
    }

    public PermissionSet Add(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException($"'{nameof(permission)}' ne peut pas avoir une valeur null ou être un espace blanc.", nameof(permission));

        return Add(new Permission(permission));
    }

    public PermissionSet Add(Permission permission)
    {
        return new PermissionSet(this.Union(Enumerable.Repeat(permission, 1)).Distinct());
    }

    public bool Allows(Permission other) => this.Any(x => x.Allows(other));

    public bool Includes(Permission other) => this.Any(x => x.Includes(other));

    public override string ToString() => display.Value;

    //public IEnumerable<string> ToIds() => this.Select(x => x.Id);
}

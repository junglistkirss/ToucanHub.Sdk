namespace Toucan.Sdk.Contracts.Security;

public readonly record struct ObjectPrivilegeSummary<T>
    where T : struct
{
    public readonly static ObjectPrivilegeSummary<T> All = new(true);
    public readonly static ObjectPrivilegeSummary<T> Empty = new(false);
    public ObjectPrivilegeSummary(params ObjectPrivilege<T>[] objects)
    {
        HasAll = false;
        Objects = objects ?? [];
    }

    private ObjectPrivilegeSummary(bool hasAll)
    {
        HasAll = hasAll;
        Objects = [];
    }

    public bool HasAll { get; }
    public ObjectPrivilege<T>[] Objects { get; }
}

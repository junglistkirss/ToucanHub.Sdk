namespace Toucan.Sdk.Contracts.Security;

public readonly record struct PrivilegeSet<T, TRight>
    where T : struct
{
    public readonly static PrivilegeSet<T, TRight> All = new(true);
    public readonly static PrivilegeSet<T, TRight> Empty = new(false);
    public PrivilegeSet(params Privilege<T, TRight>[] objects)
    {
        HasAll = false;
        Objects = objects ?? [];
    }

    private PrivilegeSet(bool hasAll)
    {
        HasAll = hasAll;
        Objects = [];
    }

    public bool HasAll { get; }
    public Privilege<T, TRight>[] Objects { get; }
}

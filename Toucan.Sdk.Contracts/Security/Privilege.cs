namespace Toucan.Sdk.Contracts.Security;

public readonly record struct Privilege<T, TRight>
    where T : struct
{
    public Privilege(T obj, TRight rights, PrivilegeSet<T, TRight>? children = null)
    {
        Obj = obj;
        Rights = rights;
        Children = children ?? PrivilegeSet<T, TRight>.Empty;
    }
    public T Obj { get; }
    public TRight Rights { get; }
    public PrivilegeSet<T, TRight> Children { get; }

}

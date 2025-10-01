namespace Toucan.Sdk.Contracts.Security;

public readonly record struct ObjectPrivilege<T>
    where T : struct
{
    public static ObjectPrivilege<T> Allow(T obj, ObjectPrivilegeSummary<T>? children = null)
    {
        return new ObjectPrivilege<T>(obj, true, children ?? ObjectPrivilegeSummary<T>.Empty);
    }
    public static ObjectPrivilege<T> Disallow(T obj, ObjectPrivilegeSummary<T>? children = null)
    {
        return new ObjectPrivilege<T>(obj, false, children ?? ObjectPrivilegeSummary<T>.Empty);
    }

    private ObjectPrivilege(T obj, bool isAllowed, ObjectPrivilegeSummary<T> children)
    {
        Obj = obj;
        IsAllowed = isAllowed;
        Children = children;
    }
    public T Obj { get; }
    public bool IsAllowed { get; }
    public ObjectPrivilegeSummary<T> Children { get; }

}

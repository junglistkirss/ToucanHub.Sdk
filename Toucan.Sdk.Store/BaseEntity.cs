using Toucan.Sdk.Contracts.Entities;
namespace Toucan.Sdk.Store;


public abstract class BaseEntity<TId> : IEntity<TId>, IEquatable<IEntity<TId>>
    where TId : struct
{
    public TId Id { get; set; } = default!;

    public override int GetHashCode() => Id!.GetHashCode();
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() == GetType())
            return Equals((IEntity<TId>)obj);

        return false;
    }

    bool IEquatable<IEntity<TId>>.Equals(IEntity<TId>? other)
    {
        if (other != null)
            return Id!.Equals(other!.Id);

        return false;
    }

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is not null && right is not null)
            return left.Equals(right);

        return false;
    }

    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right) => !(left == right);


}

namespace Yearly.Domain.Models;

public class Entity<TId> : IEquatable<Entity<TId>>
    where TId : ValueObject
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; private set; } = null!;

    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
            return false;
        
        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        
        if (ReferenceEquals(this, obj))
            return true;
        
        if (obj.GetType() != this.GetType())
            return false;
        
        return Equals((Entity<TId>)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !Equals(left, right);
    }

    protected Entity()
    {
    }
}
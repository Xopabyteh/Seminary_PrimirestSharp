namespace Yearly.Domain.Models;

public class Entity<TId> : IEquatable<Entity<TId>>
    where TId : ValueObject
    
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; private set; }

    public bool Equals(Entity<TId>? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (ReferenceEquals(this, obj))
            return true;
        
        if (obj.GetType() != this.GetType())
            return false;
        
        return Equals((Entity<TId>) obj);
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

#pragma warning disable CS8618
    protected Entity() { }
#pragma warning restore CS8618
}
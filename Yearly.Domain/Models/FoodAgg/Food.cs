using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;

namespace Yearly.Domain.Models.FoodAgg;

public sealed class Food : AggregateRoot<Guid>
{
    //public Guid? AliasForFoodId { get; private set; }
    public Guid? AliasForFoodId { get; private set; }
    
    //private readonly List<Guid> _photoIds; 
    //public List<Guid> PhotoIds => _photoIds;
    private readonly List<Photo> _photos;
    public IReadOnlyList<Photo> Photos => _photos.AsReadOnly();

    public string Name { get; private set; } 
    public string Allergens { get; private set; } // Todo: primitive obsession
    
    public PrimirestFoodIdentifier PrimirestFoodIdentifier { get; private set; }

    private Food(
        Guid id,
        Guid? aliasForFoodId,
        string name,
        string allergens,
        PrimirestFoodIdentifier primirestFoodIdentifier) 
        : base(id)
    {
        _photos = new();
        AliasForFoodId = aliasForFoodId;
        Name = name;
        Allergens = allergens;
        PrimirestFoodIdentifier = primirestFoodIdentifier;
    }

    public static Food Create(
        string name,
        string allergens,
        PrimirestFoodIdentifier primirestFoodIdentifier)
    { 
        return new Food(
            Guid.NewGuid(), 
            null,
            name,
            allergens,
            primirestFoodIdentifier);
    }

    public void UpdatePrimirestFoodIdentifier(PrimirestFoodIdentifier newPrimirestFoodIdentifier)
    {
        PrimirestFoodIdentifier = newPrimirestFoodIdentifier;
    }

#pragma warning disable CS8618 //For EF Core
    private Food()
#pragma warning restore CS8618
    {

    }
}
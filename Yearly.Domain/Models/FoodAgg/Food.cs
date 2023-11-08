using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Domain.Models.FoodAgg;

public sealed class Food : AggregateRoot<Guid>
{
    public Guid? AliasForFoodId { get; private set; }
    
    private readonly List<Guid> _photoIds; 
    public IReadOnlyList<Guid> PhotoIds => _photoIds.AsReadOnly();
    
    public string Name { get; private set; } 
    public string Allergens { get; private set; } // Todo: primitive obsession
    
    public PrimirestFoodIdentifier PrimirestFoodIdentifier { get; private set; }

    private Food(
        Guid id,
        List<Guid> photoIds,
        Guid? aliasForFoodId,
        string name,
        string allergens,
        PrimirestFoodIdentifier primirestFoodIdentifier) 
        : base(id)
    {
        _photoIds = photoIds;
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
            new List<Guid>(),
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
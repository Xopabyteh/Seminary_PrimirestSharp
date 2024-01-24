using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Domain.Models.FoodAgg;

public sealed class Food : AggregateRoot<FoodId>
{
    public FoodId? AliasForFoodId { get; private set; }
    
    private readonly List<PhotoId> _photoIds; 
    public IReadOnlyList<PhotoId> PhotoIds => _photoIds.AsReadOnly();
    
    public string Name { get; private set; } 
    public string Allergens { get; private set; } // Todo: primitive obsession
    
    public PrimirestFoodIdentifier PrimirestFoodIdentifier { get; private set; }

    private Food(
        FoodId id,
        List<PhotoId> photoIds,
        FoodId? aliasForFoodId,
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
        FoodId id,
        string name,
        string allergens,
        PrimirestFoodIdentifier primirestFoodIdentifier)
    { 
        return new Food(
            id,
            new List<PhotoId>(),
            null,
            name,
            allergens,
            primirestFoodIdentifier);
    }
    public void SetAliasForFood(Food forFood)
    {
        if (forFood.AliasForFoodId is not null)
            throw new SetAliasToFoodWithAliasException(this, forFood);
        
        AliasForFoodId = forFood.Id;
    }

#pragma warning disable CS8618 //For EF Core
    // ReSharper disable once UnusedMember.Local
    private Food()
        :base(null!)
#pragma warning restore CS8618
    {

    }
}
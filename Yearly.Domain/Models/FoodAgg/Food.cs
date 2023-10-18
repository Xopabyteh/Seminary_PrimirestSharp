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

    private Food(FoodId id, List<PhotoId> photoIds, FoodId? aliasForFoodId, string name, string allergens) 
        : base(id)
    {
        _photoIds = photoIds;
        AliasForFoodId = aliasForFoodId;
        Name = name;
        Allergens = allergens;
    }

    public static Food Create(
        string name,
        string allergens)
    { 
        return new Food(
            new FoodId(Guid.NewGuid()),
            new List<PhotoId>(),
            null,
            name,
            allergens);
    }

#pragma warning disable CS8618 //For EF Core
    private Food()
        :base(null!)
#pragma warning restore CS8618
    {

    }
}
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Domain.Models.FoodAgg;

public class Food : AggregateRoot<FoodId>
{
    public FoodId? AliasForFoodId { get; private set; }
    
    private readonly List<PhotoId> _photoIds; 
    public IReadOnlyList<PhotoId> PhotoIds => _photoIds.AsReadOnly();
    
    public DateTime Date { get; private set; } 
    public string Name { get; private set; } 
    public string Allergens { get; private set; } // Todo: primitive obsession

    protected Food(FoodId id, List<PhotoId> photoIds, FoodId? aliasForFoodId, DateTime date, string name, string allergens) 
        : base(id)
    {
        _photoIds = photoIds;
        AliasForFoodId = aliasForFoodId;
        Date = date;
        Name = name;
        Allergens = allergens;
    }
}
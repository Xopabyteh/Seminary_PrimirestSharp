using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Domain.Models.MenuAgg;

public class Menu : AggregateRoot<MenuId>
{
    private readonly List<FoodId> _foodIds;
    public IReadOnlyList<FoodId> FoodIds => _foodIds.AsReadOnly();
    
    public DateTime Date { get; private set; }

    protected Menu(MenuId id, List<FoodId> foodIds, DateTime date) : base(id)
    {
        this._foodIds = foodIds;
        this.Date = date;
    }
}
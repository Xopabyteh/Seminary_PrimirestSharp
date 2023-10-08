using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.MenuAgg.ValueObjects;

namespace Yearly.Domain.Models.MenuAgg;

public class Menu : AggregateRoot<MenuId>
{
    private readonly List<FoodId> foodIds;
    public IReadOnlyList<FoodId> FoodIds => foodIds.AsReadOnly();

    protected Menu(MenuId id, List<FoodId> foodIds) : base(id)
    {
        this.foodIds = foodIds;
    }
}
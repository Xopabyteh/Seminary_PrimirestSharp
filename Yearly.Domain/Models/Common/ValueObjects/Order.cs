using Yearly.Application.Menus;

namespace Yearly.Domain.Models.Common.ValueObjects;
public class Order : ValueObject
{
    public Guid ForFoodId { get; }
    public PrimirestFoodOrderIdentifier PrimirestOrderIdentifier { get; }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ForFoodId;
        yield return PrimirestOrderIdentifier;
    }

    public Order(Guid forFoodId, PrimirestFoodOrderIdentifier primirestOrderIdentifier)
    {
        ForFoodId = forFoodId;
        PrimirestOrderIdentifier = primirestOrderIdentifier;
    }
}

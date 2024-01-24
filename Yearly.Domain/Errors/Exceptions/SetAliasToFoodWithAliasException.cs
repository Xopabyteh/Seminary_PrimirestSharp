using Yearly.Domain.Models.FoodAgg;

namespace Yearly.Domain.Errors.Exceptions;

public class SetAliasToFoodWithAliasException : Exception
{
    public SetAliasToFoodWithAliasException(
        Food ofFood,
        Food forFood)
        : base(
            $"The food {ofFood.Id.Value} cannot be made as an alias for the food {forFood.Id.Value}, because {forFood.Id.Value} is already an alias for another food")

    {
    }
}
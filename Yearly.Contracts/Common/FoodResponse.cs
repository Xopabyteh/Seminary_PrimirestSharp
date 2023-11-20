namespace Yearly.Contracts.Common;

public record FoodResponse(
    string Name,
    string Allergens,
    List<string> PhotoLinks,
    Guid FoodId, //The id of the food in our system, not the primirest id
    PrimirestFoodIdentifierContract PrimirestFoodIdentifier
);
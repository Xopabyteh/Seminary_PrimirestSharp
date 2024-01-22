namespace Yearly.Contracts.Common;

public record FoodDTO(
    string Name,
    string Allergens,
    List<PhotoLinkDTO> PhotoLinks,
    Guid FoodId, //The id of the food in our system, not the primirest id
    PrimirestFoodIdentifierDTO PrimirestFoodIdentifier
);

public record PhotoLinkDTO(
    string ResourceLink, 
    string ThumbnailResourceLink);
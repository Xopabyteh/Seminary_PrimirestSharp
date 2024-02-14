namespace Yearly.Contracts.Foods;

public record FoodWithContextDTO(
    Guid Id,
    string Name,
    Guid? AliasForFoodId,
    string? AliasOriginName);

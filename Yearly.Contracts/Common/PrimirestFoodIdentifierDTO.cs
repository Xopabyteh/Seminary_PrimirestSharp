namespace Yearly.Contracts.Common;

public record PrimirestFoodIdentifierDTO(
    int MenuId, //Same as Week id on the primirest side.
    int DayId,
    int ItemId);
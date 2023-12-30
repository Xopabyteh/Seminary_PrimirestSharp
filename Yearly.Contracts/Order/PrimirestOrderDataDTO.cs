namespace Yearly.Contracts.Order;

public record PrimirestOrderDataDTO(
    PrimirestOrderIdentifierDTO PrimirestOrderIdentifier,
    decimal PriceCzechCrowns);

public record PrimirestOrderIdentifierDTO(
    // These three are used when canceling an order of the item
    int OrderItemId, // This is the ID of the order item
    int OrderId, // This is the ID of the order
    int MenuId // This is the ID of the menu
);
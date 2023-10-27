namespace Yearly.Contracts.Order;

public record MyOrdersResponse(
    List<OrderResponse> Orders
);


public record OrderResponse(
    // These two are used when canceling an order of the item
    int OrderItemId, // This is the ID of the order item
    int OrderId, // This is the ID of the order

    int FoodItemId // THIS POINTS TO THE FOOD ID OF THE PRIMIREST FOOD IDENTIFIER
);
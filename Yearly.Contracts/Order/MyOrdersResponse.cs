using Yearly.Contracts.Common;

namespace Yearly.Contracts.Order;

public record MyOrdersResponse(
    List<OrderResponse> Orders
);


public record OrderResponse(
    Guid SharpFoodId, //Id of the food in the sharp system
    PrimirestOrderIdentifierContract PrimirestOrderIdentifier
);
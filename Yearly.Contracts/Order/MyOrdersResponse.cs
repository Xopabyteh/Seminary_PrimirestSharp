using Yearly.Contracts.Common;

namespace Yearly.Contracts.Order;

public record MyOrdersResponse(
    List<OrderDTO> Orders
);


public record OrderDTO(
    Guid SharpFoodId, //Id of the food in the sharp system
    PrimirestOrderIdentifierDTO PrimirestOrderIdentifier
);
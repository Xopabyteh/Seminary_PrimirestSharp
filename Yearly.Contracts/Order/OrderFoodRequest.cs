using Yearly.Contracts.Common;

namespace Yearly.Contracts.Order;

public record OrderFoodRequest(
    string SessionCookie,
    PrimirestFoodIdentifierResponse PrimirestFoodIdentifier);
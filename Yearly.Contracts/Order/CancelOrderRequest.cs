using Yearly.Contracts.Common;

namespace Yearly.Contracts.Order;

public readonly record struct CancelOrderRequest(PrimirestOrderIdentifierContract PrimirestOrderIdentifier);

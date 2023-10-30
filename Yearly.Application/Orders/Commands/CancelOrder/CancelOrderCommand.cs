using ErrorOr;
using MediatR;
using Yearly.Application.Menus;

namespace Yearly.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand(PrimirestFoodOrderIdentifier PrimirestFoodOrderIdentifier, string SessionCookie)
    : IRequest<ErrorOr<Unit>>;
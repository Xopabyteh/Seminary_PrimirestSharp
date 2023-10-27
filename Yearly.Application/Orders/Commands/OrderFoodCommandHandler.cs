using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Application.Orders.Commands;

public class OrderFoodCommandHandler : IRequestHandler<OrderFoodCommand, ErrorOr<Unit>>
{
    private IPrimirestOrderService _primirestOrderService;
    public OrderFoodCommandHandler(IPrimirestOrderService primirestOrderService)
    {
        _primirestOrderService = primirestOrderService;
    }

    public Task<ErrorOr<Unit>> Handle(OrderFoodCommand request, CancellationToken cancellationToken)
    {
        return _primirestOrderService.OrderFoodAsync(request.SessionCookie, request.PrimirestFoodIdentifier);
    }
}
using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Application.Menus;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Orders.Commands;

public record OrderFoodCommand(string SessionCookie, PrimirestFoodIdentifier PrimirestFoodIdentifier)
    : IRequest<ErrorOr<PrimirestFoodOrderIdentifier>>;

public class OrderFoodCommandHandler : IRequestHandler<OrderFoodCommand, ErrorOr<PrimirestFoodOrderIdentifier>>
{
    private readonly IPrimirestOrderService _primirestOrderService;
    public OrderFoodCommandHandler(IPrimirestOrderService primirestOrderService)
    {
        _primirestOrderService = primirestOrderService;
    }

    public Task<ErrorOr<PrimirestFoodOrderIdentifier>> Handle(OrderFoodCommand request, CancellationToken cancellationToken)
    {
        return _primirestOrderService.OrderFoodAsync(request.SessionCookie, request.PrimirestFoodIdentifier);
    }
}
using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Orders.Commands;

public record OrderFoodCommand(string SessionCookie, PrimirestFoodIdentifier PrimirestFoodIdentifier)
    : IRequest<ErrorOr<Unit>>;

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
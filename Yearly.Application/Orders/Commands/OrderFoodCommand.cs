using ErrorOr;
using MediatR;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Orders.Commands;

public record OrderFoodCommand(string SessionCookie, PrimirestFoodIdentifier PrimirestFoodIdentifier)
    : IRequest<ErrorOr<PrimirestOrderData>>;

public class OrderFoodCommandHandler : IRequestHandler<OrderFoodCommand, ErrorOr<PrimirestOrderData>>
{
    private readonly IPrimirestOrderService _primirestOrderService;
    public OrderFoodCommandHandler(IPrimirestOrderService primirestOrderService)
    {
        _primirestOrderService = primirestOrderService;
    }

    public Task<ErrorOr<PrimirestOrderData>> Handle(OrderFoodCommand request, CancellationToken cancellationToken)
    {
        return _primirestOrderService.OrderFoodAsync(request.SessionCookie, request.PrimirestFoodIdentifier);
    }
}
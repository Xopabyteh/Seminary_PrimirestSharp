using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, ErrorOr<Unit>>
{
    private readonly IPrimirestOrderService _primirestOrderService;

    public CancelOrderCommandHandler(IPrimirestOrderService primirestOrderService)
    {
        _primirestOrderService = primirestOrderService;
    }

    public Task<ErrorOr<Unit>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        return _primirestOrderService.CancelOrderAsync(request.SessionCookie, request.PrimirestFoodOrderIdentifier);
    }
}
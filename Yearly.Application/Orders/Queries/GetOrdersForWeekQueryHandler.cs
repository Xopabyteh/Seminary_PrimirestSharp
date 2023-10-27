using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Application.Menus;

namespace Yearly.Application.Orders.Queries;

public class GetOrdersForWeekQueryHandler : IRequestHandler<GetOrdersForWeekQuery, ErrorOr<IReadOnlyList<PrimirestFoodOrder>>>
{
    private readonly IPrimirestOrderService _primirestOrderService;

    public GetOrdersForWeekQueryHandler(IPrimirestOrderService primirestOrderService)
    {
        _primirestOrderService = primirestOrderService;
    }

    public Task<ErrorOr<IReadOnlyList<PrimirestFoodOrder>>> Handle(GetOrdersForWeekQuery request, CancellationToken cancellationToken)
    {
        var ordersResult = _primirestOrderService.GetOrdersForPersonForWeekAsync(request.SessionCookie, request.WeekId);
        return ordersResult;
    }
}
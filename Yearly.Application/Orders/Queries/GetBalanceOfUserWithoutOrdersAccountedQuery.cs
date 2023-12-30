using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Orders.Queries;

public record GetBalanceOfUserWithoutOrdersAccountedQuery(string SessionCookie, UserId UserId) : IRequest<ErrorOr<MoneyCzechCrowns>>;

public class GetBalanceOfUserWithoutOrdersAccountedQueryHandler 
    : IRequestHandler<GetBalanceOfUserWithoutOrdersAccountedQuery, ErrorOr<MoneyCzechCrowns>>
{
    private readonly IPrimirestOrderService _primirestOrderService;
    public GetBalanceOfUserWithoutOrdersAccountedQueryHandler(IPrimirestOrderService primirestOrderService)
    {
        _primirestOrderService = primirestOrderService;
    }

    public Task<ErrorOr<MoneyCzechCrowns>> Handle(
        GetBalanceOfUserWithoutOrdersAccountedQuery request,
        CancellationToken cancellationToken)
    {
        var balance = _primirestOrderService.GetBalanceOfUserWithoutOrdersAccountedAsync(
            request.SessionCookie,
            request.UserId);

        return balance;
    }
}

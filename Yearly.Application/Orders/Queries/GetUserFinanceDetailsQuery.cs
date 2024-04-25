using ErrorOr;
using MediatR;
using Yearly.Domain.Models.Common.ValueObjects;

namespace Yearly.Application.Orders.Queries;

public record GetUserFinanceDetailsQuery(string sessionCookie)
    : IRequest<ErrorOr<UserFinanceDetails>>;

public class GetUserFinanceDetailsQueryHandler : IRequestHandler<GetUserFinanceDetailsQuery, ErrorOr<UserFinanceDetails>>
{
    private readonly IPrimirestFinanceService _primirestFinanceService;

    public GetUserFinanceDetailsQueryHandler(IPrimirestFinanceService primirestFinanceService)
    {
        _primirestFinanceService = primirestFinanceService;
    }

    public Task<ErrorOr<UserFinanceDetails>> Handle(GetUserFinanceDetailsQuery request, CancellationToken cancellationToken)
    {
        return _primirestFinanceService.GetFinanceDetailsForUser(request.sessionCookie);
    }
}

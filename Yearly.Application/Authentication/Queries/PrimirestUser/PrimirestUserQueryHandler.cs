using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Application.Authentication.Queries.PrimirestUser;

public class PrimirestUserQueryHandler : IRequestHandler<PrimirestUserQuery, ErrorOr<PrimirestUser>>
{
    private readonly IPrimirestAuthService _primirestAuthService;

    public PrimirestUserQueryHandler(IPrimirestAuthService primirestAuthService)
    {
        _primirestAuthService = primirestAuthService;
    }

    public Task<ErrorOr<PrimirestUser>> Handle(PrimirestUserQuery request, CancellationToken cancellationToken)
    {
        return _primirestAuthService.GetPrimirestUserInfoAsync(request.SessionCookie);
    }
}
using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Application.Authentication.Queries.PrimirestUser;

public class PrimirestUserQueryHandler : IRequestHandler<PrimirestUserQuery, ErrorOr<PrimirestUser>>
{
    private readonly IAuthService _authService;

    public PrimirestUserQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<ErrorOr<PrimirestUser>> Handle(PrimirestUserQuery request, CancellationToken cancellationToken)
    {
        return _authService.GetUserInfoAsync(request.SessionCookie);
    }
}
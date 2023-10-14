using MediatR;
using Yearly.Application.Authentication.Queries.Logout;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Application.Authentication.Queries.Login;

public class LogoutQueryHandler : IRequestHandler<LogoutQuery>
{
    private readonly IAuthService _authService;

    public LogoutQueryHandler(IAuthService authService)
    {
        this._authService = authService;
    }

    public async Task Handle(LogoutQuery request, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request.SessionCookie);
    }
}
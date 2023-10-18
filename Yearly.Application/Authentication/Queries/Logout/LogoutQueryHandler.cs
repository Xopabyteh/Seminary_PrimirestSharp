using MediatR;
using Yearly.Application.Authentication.Queries.Logout;

namespace Yearly.Application.Authentication.Queries.Login;

public class LogoutQueryHandler : IRequestHandler<LogoutQuery>
{
    private readonly IExternalAuthService _externalAuthService;

    public LogoutQueryHandler(IExternalAuthService externalAuthService)
    {
        this._externalAuthService = externalAuthService;
    }

    public async Task Handle(LogoutQuery request, CancellationToken cancellationToken)
    {
        await _externalAuthService.LogoutAsync(request.SessionCookie);
    }
}
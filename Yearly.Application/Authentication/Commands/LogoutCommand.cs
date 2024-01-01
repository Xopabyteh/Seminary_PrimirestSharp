using MediatR;

namespace Yearly.Application.Authentication.Commands;

public record LogoutCommand(string SessionCookie) : IRequest;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAuthService _authService;
    private readonly ISessionCache _sessionCache;

    public LogoutCommandHandler(IAuthService authService, ISessionCache sessionCache)
    {
        _authService = authService;
        _sessionCache = sessionCache;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _sessionCache.RemoveAsync(request.SessionCookie);

        await _authService.LogoutAsync(request.SessionCookie);
    }
}
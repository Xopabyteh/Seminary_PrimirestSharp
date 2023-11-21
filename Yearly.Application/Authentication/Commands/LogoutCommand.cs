using MediatR;

namespace Yearly.Application.Authentication.Commands;

public record LogoutCommand(string SessionCookie) : IRequest;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request.SessionCookie);
    }
}
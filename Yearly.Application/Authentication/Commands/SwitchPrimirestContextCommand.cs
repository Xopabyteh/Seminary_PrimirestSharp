using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication.Commands;

public sealed record SwitchPrimirestContextCommand(string SessionCookie, UserId NewUserId) : IRequest<ErrorOr<Unit>>;

internal sealed class SwitchPrimirestContextCommandHandler
    : IRequestHandler<SwitchPrimirestContextCommand, ErrorOr<Unit>>
{
    private readonly IAuthService _authService;

    public SwitchPrimirestContextCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<ErrorOr<Unit>> Handle(SwitchPrimirestContextCommand request, CancellationToken cancellationToken)
    {
        return _authService.SwitchPrimirestContextAsync(request.SessionCookie, request.NewUserId);
    }
}
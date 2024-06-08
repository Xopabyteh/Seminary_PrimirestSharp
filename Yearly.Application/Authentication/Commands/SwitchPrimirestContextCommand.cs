using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Commands;

/// <summary>
/// Returns new session expiration time
/// </summary>
/// <param name="SessionCookie"></param>
/// <param name="NewUserId"></param>
public sealed record SwitchPrimirestContextCommand(string SessionCookie, UserId NewUserId) 
    : IRequest<ErrorOr<DateTimeOffset>>;

internal sealed class SwitchPrimirestContextCommandHandler
    : IRequestHandler<SwitchPrimirestContextCommand, ErrorOr<DateTimeOffset>>
{
    private readonly IAuthService _authService;
    private readonly ISessionCache _sessionCache;
    private readonly IUserRepository _userRepository;

    public SwitchPrimirestContextCommandHandler(IAuthService authService, ISessionCache sessionCache, IUserRepository userRepository)
    {
        _authService = authService;
        _sessionCache = sessionCache;
        _userRepository = userRepository;
    }

    /// <returns>New session expiration time</returns>
    public async Task<ErrorOr<DateTimeOffset>> Handle(SwitchPrimirestContextCommand request, CancellationToken cancellationToken)
    {
        var newUser = await _userRepository.GetByIdAsync(request.NewUserId);
        if (newUser is null)
            return Errors.Errors.User.UserNotFound;

        var contextSwitchResult = await _authService.SwitchPrimirestContextAsync(request.SessionCookie, request.NewUserId);
        if (contextSwitchResult.IsError)
            return contextSwitchResult.Errors;

        var newExpirationTime = await _sessionCache.SetAsync(request.SessionCookie, newUser);
        return newExpirationTime;
    }
}
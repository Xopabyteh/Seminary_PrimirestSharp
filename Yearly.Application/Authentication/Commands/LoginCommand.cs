using ErrorOr;
using FluentValidation;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Errors.Exceptions;

namespace Yearly.Application.Authentication.Commands;

public record LoginCommand(string Username, string Password) : IRequest<ErrorOr<LoginResult>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<LoginResult>>
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionCache _sessionCache;
    private readonly IUserOnboarderService _userOnboarderService;
    public LoginCommandHandler(IAuthService authService, IUnitOfWork unitOfWork, ISessionCache sessionCache, IUserOnboarderService userOnboarderService)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _sessionCache = sessionCache;
        _userOnboarderService = userOnboarderService;
    }

    public async Task<ErrorOr<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var sessionCookieResult = await _authService.LoginAsync(request.Username, request.Password);

        if (sessionCookieResult.IsError)
            return sessionCookieResult.Errors;

        var externalUserInfoAsync = await _authService.GetAvailableUsersInfoAsync(sessionCookieResult.Value);
        if (externalUserInfoAsync.IsError)
        {
            throw new IllegalStateException("Cannot retrieve external info of a user that just logged in");
        }

        // Users from primirest
        var availableExternalUsersDetails = externalUserInfoAsync.Value;

        // Onboard/update users in our system
        var availableSharpUsers  = await _userOnboarderService.OnboardOrUpdatePrimirestUsersToSharp(availableExternalUsersDetails);
        if(availableSharpUsers.IsError)
            return availableSharpUsers.Errors;

        // Save
        await _unitOfWork.SaveChangesAsync();

        // Make the first user the active
        var activeLoggedUser = availableSharpUsers.Value.First();
        var sessionExpirationTime = await _sessionCache.SetAsync(sessionCookieResult.Value, activeLoggedUser);

        if (availableSharpUsers.Value.Count > 1)
        {
            // Less common case:
            // There are multiple available users, so we need to
            // explicitly switch the context to the active one on primirest
            await _authService.SwitchPrimirestContextAsync(sessionCookieResult.Value, activeLoggedUser.Id);
        }

        return new LoginResult(
            activeLoggedUser,
            availableSharpUsers.Value,
            sessionCookieResult.Value,
            sessionExpirationTime);
    }
}
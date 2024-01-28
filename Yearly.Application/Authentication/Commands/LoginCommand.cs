using ErrorOr;
using FluentValidation;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

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
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionCache _sessionCache;
    public LoginCommandHandler(IAuthService authService, IUserRepository userRepository, IUnitOfWork unitOfWork, ISessionCache sessionCache)
    {
        _authService = authService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _sessionCache = sessionCache;
    }

    public async Task<ErrorOr<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
#if DEBUG
        if (request.Username == "debug")
        {
            var debugUserId = await _sessionCache.GetAsync("debug");
            var debugUser = await _userRepository.GetByIdAsync(debugUserId!);
            var result = new LoginResult("debug", debugUser!);
            return result;
        }
#endif

        var externalLoginResult = await _authService.LoginAsync(request.Username, request.Password);

        if (externalLoginResult.IsError)
            return externalLoginResult.Errors;

        var externalUserInfoAsync = await _authService.GetPrimirestUserInfoAsync(externalLoginResult.Value);
        if (externalUserInfoAsync.IsError)
        {
            throw new IllegalStateException("Cannot retrieve external info of a user that just logged in");

            //externalLoginResult.Errors.AddRange(externalUserInfoAsync.Errors);
            //return externalLoginResult.Errors;
        }

        var externalUser = externalUserInfoAsync.Value;

        //Get user from our system
        var sharpUser = await _userRepository.GetByIdAsync(new UserId(externalUser.Id));
        if (sharpUser is null)
        {
            //Persist user in our db
            sharpUser = new User(new UserId(externalUser.Id), externalUser.Username);

            await _userRepository.AddAsync(sharpUser);
            await _unitOfWork.SaveChangesAsync();
        }

        //Add to cache
        await _sessionCache.AddAsync(externalLoginResult.Value, sharpUser.Id);

        return new LoginResult(externalLoginResult.Value, sharpUser);
    }
}

public record LoginResult(string SessionCookie, User User);
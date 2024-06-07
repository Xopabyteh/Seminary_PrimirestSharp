﻿using ErrorOr;
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
        var externalLoginResult = await _authService.LoginAsync(request.Username, request.Password);

        if (externalLoginResult.IsError)
            return externalLoginResult.Errors;

        var externalUserInfoAsync = await _authService.GetPrimirestUserInfoAsync(externalLoginResult.Value);
        if (externalUserInfoAsync.IsError)
        {
            throw new IllegalStateException("Cannot retrieve external info of a user that just logged in");
        }

        // Users from primirest
        var availableExternalUsersDetails = externalUserInfoAsync.Value;

        // Users from sharp (might not be in our system yet)
        var availableSharpUsers = await _userRepository.GetUsersByIdsAsync(availableExternalUsersDetails
            .Select(d => new UserId(d.Id))
            .ToArray());

        // Onboard nonexistant users
        var didOnboardAnyone = false;
        foreach (var externalUserDetails in availableExternalUsersDetails)
        {
            if (!availableSharpUsers.TryGetValue(new UserId(externalUserDetails.Id), out var sharpUser))
            {
                // Onboard
                sharpUser = new User(new UserId(externalUserDetails.Id), externalUserDetails.Username);

                // Add to dictionary
                availableSharpUsers.Add(sharpUser.Id, sharpUser);

                // Persist user in sharp db
                await _userRepository.AddAsync(sharpUser);
                didOnboardAnyone = true;
            }
        }
        
        // Save onboarding
        if(didOnboardAnyone)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        if(availableSharpUsers.Count == 0)
        {
            throw new IllegalStateException("No users were translated to sharp");
        }
        if(availableSharpUsers.Count == 1)
        {
            // Common case, user is only in one tenant
            var sharpUser = availableSharpUsers.First();

            //Add to cache
            await _sessionCache.AddAsync(externalLoginResult.Value, sharpUser.Value);
            
            return new LoginResult(
                [new(externalLoginResult.Value, sharpUser.Value)],
                _sessionCache.SessionExpiration);
        }

        // -> Multiple users within tenant
        var sharpUsersDetails = availableSharpUsers.Values
            .Select(u => new UserDetailsResult(externalLoginResult.Value, u))
            .ToArray();

        return new LoginResult(
            sharpUsersDetails,
            _sessionCache.SessionExpiration);
    }
}

public record LoginResult(UserDetailsResult[] availableSharpUsers, DateTimeOffset SessionExpirationTime);
public record UserDetailsResult(string SessionCookie, User User);
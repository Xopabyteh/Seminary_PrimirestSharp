using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<LoginResult>>
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginQueryHandler(IAuthService authService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        this._authService = authService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<LoginResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var externalLoginResult = await _authService.LoginAsync(request.Username, request.Password);

        //TODO: Change this to not be in a query
        //Check if it's first login, if yes, persist user
        if (externalLoginResult.IsError)
            return externalLoginResult.Errors;

        var externalUserInfoAsync = await _authService.GetExternalUserInfoAsync(externalLoginResult.Value);
        if (externalUserInfoAsync.IsError)
        {
            throw new IllegalStateException("Cannot retrieve external info of a user that just logged in");

            //externalLoginResult.Errors.AddRange(externalUserInfoAsync.Errors);
            //return externalLoginResult.Errors;
        }

        var externalUser = externalUserInfoAsync.Value;

        //Get user from our system
        var user = await _userRepository.GetByIdAsync(new UserId(externalUser.Id));
        if (user is null)
        {
            //Persist user in our db
            user = new User(new UserId(externalUser.Id), externalUser.Username);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        return new LoginResult(externalLoginResult.Value, user);
    }
}
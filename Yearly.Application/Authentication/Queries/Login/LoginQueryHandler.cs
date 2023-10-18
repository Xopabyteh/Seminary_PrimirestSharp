using ErrorOr;
using MediatR;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<LoginResult>>
{
    private IExternalAuthService _externalAuthService;
    private IUserRepository _userRepository;

    public LoginQueryHandler(IExternalAuthService externalAuthService, IUserRepository userRepository)
    {
        this._externalAuthService = externalAuthService;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<LoginResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var loginResult = await _externalAuthService.LoginAsync(request.Username, request.Password);

        //TODO: Change this to not be in a query
        //Check if it's first login, if yes, persist user
        if (!loginResult.IsError)
        {
            var userInfoResult = await _externalAuthService.GetUserInfoAsync(loginResult.Value.SessionCookie);
            if (userInfoResult.IsError)
            {
                loginResult.Errors.AddRange(userInfoResult.Errors);
                return loginResult;
            }

            var user = userInfoResult.Value;
            var userExists = await _userRepository.DoesUserExistAsync(user.Username);
            if (!userExists)
            {
                await _userRepository.AddAsync(user);
            }
        }

        return loginResult;
    }
}
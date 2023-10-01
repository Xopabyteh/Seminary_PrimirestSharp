using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<LoginResult>>
{
    private IPrimirestAuthService _primirestAuthService;

    public LoginQueryHandler(IPrimirestAuthService primirestAuthService)
    {
        _primirestAuthService = primirestAuthService;
    }

    public async Task<ErrorOr<LoginResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = await _primirestAuthService.LoginAsync(request.Username, request.Password);
        return result;
    }
}
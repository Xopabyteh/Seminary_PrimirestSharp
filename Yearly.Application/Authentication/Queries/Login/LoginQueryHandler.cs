using ErrorOr;
using MediatR;

namespace Yearly.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<LoginResult>>
{
    private IAuthService authService;

    public LoginQueryHandler(IAuthService authService)
    {
        this.authService = authService;
    }

    public async Task<ErrorOr<LoginResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Username, request.Password);
        return result;
    }
}
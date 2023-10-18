using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Queries.PrimirestUser;

public class UserQueryHandler : IRequestHandler<UserQuery, ErrorOr<User>>
{
    private readonly IAuthService _authService;

    public UserQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<ErrorOr<User>> Handle(UserQuery request, CancellationToken cancellationToken)
    {
        return _authService.GetUserInfoAsync(request.SessionCookie);
    }
}
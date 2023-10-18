using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Queries.PrimirestUser;

public class UserQueryHandler : IRequestHandler<UserQuery, ErrorOr<User>>
{
    private readonly IExternalAuthService _externalAuthService;

    public UserQueryHandler(IExternalAuthService externalAuthService)
    {
        _externalAuthService = externalAuthService;
    }

    public Task<ErrorOr<User>> Handle(UserQuery request, CancellationToken cancellationToken)
    {
        return _externalAuthService.GetUserInfoAsync(request.SessionCookie);
    }
}
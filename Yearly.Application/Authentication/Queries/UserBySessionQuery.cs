using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Queries;

public record UserBySessionQuery(string SessionCookie) : IRequest<ErrorOr<User>>;

public class UserBySessionQueryHandler : IRequestHandler<UserBySessionQuery, ErrorOr<User>>
{
    private readonly IAuthService _authService;

    public UserBySessionQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<ErrorOr<User>> Handle(UserBySessionQuery request, CancellationToken cancellationToken)
    {
        return _authService.GetSharpUserAsync(request.SessionCookie);
    }
}
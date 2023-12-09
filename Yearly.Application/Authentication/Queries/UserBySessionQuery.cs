using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Queries;

public record UserBySessionQuery(string SessionCookie) : IRequest<ErrorOr<User>>;

public class UserBySessionQueryHandler : IRequestHandler<UserBySessionQuery, ErrorOr<User>>
{
    private readonly ISessionCache _sessionCache;

    public UserBySessionQueryHandler(ISessionCache sessionCache)
    {
        _sessionCache = sessionCache;
    }

    public async Task<ErrorOr<User>> Handle(UserBySessionQuery request, CancellationToken cancellationToken)
    {
        var user = _sessionCache.Get(request.SessionCookie);

        if (user is null)
            return Errors.Errors.Authentication.CookieNotSigned;

        return user!;
    }
}
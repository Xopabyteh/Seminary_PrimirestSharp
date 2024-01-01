using ErrorOr;
using MediatR;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Queries;

public record UserBySessionQuery(string SessionCookie) : IRequest<ErrorOr<User>>;

public class UserBySessionQueryHandler : IRequestHandler<UserBySessionQuery, ErrorOr<User>>
{
    private readonly ISessionCache _sessionCache;
    private readonly IUserRepository _userRepository;

    public UserBySessionQueryHandler(ISessionCache sessionCache, IUserRepository userRepository)
    {
        _sessionCache = sessionCache;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<User>> Handle(UserBySessionQuery request, CancellationToken cancellationToken)
    {
        var userId = await _sessionCache.GetAsync(request.SessionCookie);

        if (userId is null)
            return Errors.Errors.Authentication.SessionNotCached;

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            throw new IllegalStateException(
                $"UserId - {userId.Value} is cached, but User with that id is not present in our repository");

        return user;
    }
}
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Commands;

/// <param name="ActiveLoggedUser">The user that is now in the session cache</param>
/// <param name="AvailableUsers">All available users within a "user tenant"</param>
public record LoginResult(
    User ActiveLoggedUser,
    IReadOnlyCollection<User> AvailableUsers,
    string SessionCookie,
    DateTimeOffset SessionExpirationTime);
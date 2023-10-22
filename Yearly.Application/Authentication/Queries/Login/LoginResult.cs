using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Queries.Login;

public record LoginResult(string SessionCookie, User User);
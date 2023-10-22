using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Commands.Login;

public record LoginResult(string SessionCookie, User User);
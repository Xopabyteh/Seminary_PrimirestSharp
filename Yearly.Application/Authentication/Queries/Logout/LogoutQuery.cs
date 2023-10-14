using MediatR;

namespace Yearly.Application.Authentication.Queries.Logout;

public record LogoutQuery(string SessionCookie) : IRequest;

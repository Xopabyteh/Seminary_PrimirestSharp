using ErrorOr;
using MediatR;
using Yearly.Application.Authentication.Queries.Login;

namespace Yearly.Application.Authentication.Queries.Logout;

public record LogoutQuery(string sessionCookie) : IRequest;

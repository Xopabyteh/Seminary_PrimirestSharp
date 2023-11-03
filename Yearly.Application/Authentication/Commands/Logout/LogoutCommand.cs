using MediatR;

namespace Yearly.Application.Authentication.Commands.Logout;

public record LogoutCommand(string SessionCookie) : IRequest;

using ErrorOr;
using MediatR;

namespace Yearly.Application.Authentication.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<ErrorOr<LoginResult>>;
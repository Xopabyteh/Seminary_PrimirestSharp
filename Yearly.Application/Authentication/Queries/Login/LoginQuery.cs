using ErrorOr;
using MediatR;

namespace Yearly.Application.Authentication.Queries.Login;

public record LoginQuery(string Username, string Password) : IRequest<ErrorOr<LoginResult>>;
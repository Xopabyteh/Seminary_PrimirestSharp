using ErrorOr;
using MediatR;

namespace Yearly.Application.Authentication.Queries.PrimirestUser;

public record PrimirestUserQuery(string SessionCookie) : IRequest<ErrorOr<PrimirestUser>>;

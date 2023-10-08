using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg;

namespace Yearly.Application.Authentication.Queries.PrimirestUser;

public record UserQuery(string SessionCookie) : IRequest<ErrorOr<User>>;

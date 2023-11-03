using ErrorOr;
using MediatR;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication.Commands.Roles;

public record AddRoleToUserCommand(UserId UserId, UserRole Role) : IRequest<ErrorOr<Unit>>;
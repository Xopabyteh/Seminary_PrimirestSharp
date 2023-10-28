using FluentValidation;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Application.Authentication.Commands.Roles;

public class AddRoleToUserCommandValidator : AbstractValidator<AddRoleToUserCommand>
{
    public AddRoleToUserCommandValidator()
    {
        RuleFor(x => x.SessionCookie).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Role)
            .Must(r => UserRole.IsKnown(r.RoleCode))
            .WithMessage("Unknown role code");
    }
}
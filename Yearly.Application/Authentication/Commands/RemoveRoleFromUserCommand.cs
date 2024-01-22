using ErrorOr;
using FluentValidation;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Commands;

public record RemoveRoleFromUserCommand(User Issuer, UserId UserId, UserRole Role) : IRequest<ErrorOr<Unit>>;

public class RemoveRoleFromUserCommandValidator : AbstractValidator<AddRoleToUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Role)
            .Must(r => UserRole.IsKnown(r.RoleCode))
            .WithMessage("Unknown role code");
    }
}

public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleFromUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            return Errors.Errors.User.UserNotFound;

        var admin = Admin.FromUser(request.Issuer);
        admin.RemoveRole(request.Role, user);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
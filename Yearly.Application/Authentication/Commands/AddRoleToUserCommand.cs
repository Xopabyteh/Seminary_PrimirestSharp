using ErrorOr;
using FluentValidation;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Commands;

public record AddRoleToUserCommand(UserId UserId, UserRole Role) : IRequest<ErrorOr<Unit>>;

public class AddRoleToUserCommandValidator : AbstractValidator<AddRoleToUserCommand>
{
    public AddRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Role)
            .Must(r => UserRole.IsKnown(r.RoleCode))
            .WithMessage("Unknown role code");
    }
}

public class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddRoleToUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            return Errors.Errors.User.UserNotFound;

        user.AddRole(request.Role);

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
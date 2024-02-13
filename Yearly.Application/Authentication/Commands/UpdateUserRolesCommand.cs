using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Commands;

public readonly record struct UpdateUserRolesCommand(UserId OfUser, List<UserRole> NewRoles, User Issuer) : IRequest<ErrorOr<Unit>>;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRolesCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.OfUser);
        if (user is null)
            return Errors.Errors.User.UserNotFound;

        var admin = Admin.FromUser(request.Issuer);
        admin.UpdateRoles(user, request.NewRoles);

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
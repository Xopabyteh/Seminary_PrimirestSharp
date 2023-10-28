using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Authentication.Commands.Roles
{
    public class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand, ErrorOr<Unit>>
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddRoleToUserCommandHandler(IAuthService authService, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Unit>> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var issuerResult = await _authService.GetSharpUserAsync(request.SessionCookie);
            if (issuerResult.IsError)
                return issuerResult.Errors;

            var issuer = issuerResult.Value;
            if (!issuer.Roles.Contains(UserRole.Admin))
                return Errors.Errors.Authentication.InsufficientPermissions;

            var user = await _userRepository.GetByIdAsync(request.UserId);
            
            if (user is null)
                return Errors.Errors.User.UserNotFound;

            user.AddRole(request.Role);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
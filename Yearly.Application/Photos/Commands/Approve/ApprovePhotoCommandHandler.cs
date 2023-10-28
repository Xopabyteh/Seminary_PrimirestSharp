using ErrorOr;
using MediatR;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Commands.Approve;

public class ApprovePhotoCommandHandler : IRequestHandler<ApprovePhotoCommand, ErrorOr<Unit>>
{
    private readonly IAuthService _authService;
    private readonly IPhotoRepository _photoRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ApprovePhotoCommandHandler(IAuthService authService, IPhotoRepository photoRepository, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(ApprovePhotoCommand request, CancellationToken cancellationToken)
    {
        var userResult =  await _authService.GetSharpUserAsync(request.SessionCookie);
        if(userResult.IsError)
            return userResult.Errors;

        var user = userResult.Value;
        if (!user.Roles.Contains(UserRole.PhotoApprover))
            return Errors.Errors.Authentication.InsufficientPermissions;

        var photo = await _photoRepository.GetAsync(request.PhotoId);
        if(photo is null)
            return Errors.Errors.Photo.PhotoNotFound;

        user.ApprovePhoto(photo);
        
        await _photoRepository.UpdatePhotoAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
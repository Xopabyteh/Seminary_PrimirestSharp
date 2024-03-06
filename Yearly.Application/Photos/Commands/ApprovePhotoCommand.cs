using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Contracts.Notifications;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Commands;

public record ApprovePhotoCommand(PhotoId PhotoId, User Issuer) : IRequest<ErrorOr<Unit>>;

public class ApprovePhotoCommandHandler : IRequestHandler<ApprovePhotoCommand, ErrorOr<Unit>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserNotificationService _notificationService; //Todo: consider using events

    public ApprovePhotoCommandHandler(IPhotoRepository photoRepository, IUnitOfWork unitOfWork, IUserNotificationService notificationService)
    {
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<ErrorOr<Unit>> Handle(ApprovePhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);
        if (photo is null)
            return Errors.Errors.Photo.PhotoNotFound;

        var photoApprover = PhotoApprover.FromUser(request.Issuer);
        photoApprover.ApprovePhoto(photo);

        await _photoRepository.UpdatePhotoAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        await _notificationService.SendPushNotificationAsync(
            "Úspěch",
            "Vaše fotka byla schválena",
            photo.PublisherId, 
            NotificationTagContract.PhotoApproved);

        return Unit.Value;
    }
}
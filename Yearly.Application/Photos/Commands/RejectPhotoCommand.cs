using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Commands;

public record RejectPhotoCommand(PhotoId PhotoId, User Issuer) : IRequest<ErrorOr<Unit>>;

public class RejectPhotoCommandHandler : IRequestHandler<RejectPhotoCommand, ErrorOr<Unit>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IUnitOfWork _unitOfWork;
    public RejectPhotoCommandHandler(IPhotoRepository photoRepository, IUnitOfWork unitOfWork, IPhotoStorage photoStorage)
    {
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
        _photoStorage = photoStorage;
    }

    public async Task<ErrorOr<Unit>> Handle(RejectPhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);

        if (photo is null)
            return Errors.Errors.Photo.PhotoNotFound;

        var photoApprover = PhotoApprover.FromUser(request.Issuer);
        photoApprover.RejectPhoto(photo);

        await _photoStorage.DeletePhotoAsync(photo.ResourceLink);
        await _photoRepository.DeletePhotoAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
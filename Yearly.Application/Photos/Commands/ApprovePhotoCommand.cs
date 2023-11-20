using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Commands;

public record ApprovePhotoCommand(PhotoId PhotoId, User Approver) : IRequest<ErrorOr<Unit>>;

public class ApprovePhotoCommandHandler : IRequestHandler<ApprovePhotoCommand, ErrorOr<Unit>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ApprovePhotoCommandHandler(IPhotoRepository photoRepository, IUnitOfWork unitOfWork)
    {
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(ApprovePhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);
        if (photo is null)
            return Errors.Errors.Photo.PhotoNotFound;

        request.Approver.ApprovePhoto(photo);

        await _photoRepository.UpdatePhotoAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
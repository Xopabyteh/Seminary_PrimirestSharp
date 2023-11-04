using ErrorOr;
using MediatR;
using Yearly.Application.Authentication;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Commands.Reject;

public class RejectPhotoCommandHandler : IRequestHandler<RejectPhotoCommand, ErrorOr<Unit>>
{
    private readonly IAuthService _authService;
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IUnitOfWork _unitOfWork;
    public RejectPhotoCommandHandler(IAuthService authService, IPhotoRepository photoRepository, IUnitOfWork unitOfWork, IPhotoStorage photoStorage)
    {
        _authService = authService;
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
        _photoStorage = photoStorage;
    }

    public async Task<ErrorOr<Unit>> Handle(RejectPhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);

        if(photo is null)
            return Errors.Errors.Photo.PhotoNotFound;

        request.Rejector.RejectPhoto(photo);

        await _photoStorage.DeletePhotoAsync(Photo.NameFrom(photo.Id, photo.FoodId));
        await _photoRepository.DeletePhotoAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
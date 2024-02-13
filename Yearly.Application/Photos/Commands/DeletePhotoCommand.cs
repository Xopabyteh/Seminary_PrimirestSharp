using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.Commands;

public readonly record struct DeletePhotoCommand(PhotoId Id) : IRequest<ErrorOr<Unit>>;

public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, ErrorOr<Unit>>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePhotoCommandHandler(IPhotoRepository photoRepository, IPhotoStorage photoStorage, IUnitOfWork unitOfWork)
    {
        _photoRepository = photoRepository;
        _photoStorage = photoStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.Id);
        if (photo is null)
            return Errors.Errors.Photo.PhotoNotFound;

        await _photoStorage.DeletePhotoAsync(photo.ResourceLink);
        await _photoStorage.DeletePhotoAsync(photo.ThumbnailResourceLink);

        await _photoRepository.DeletePhotoAsync(photo);

        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}
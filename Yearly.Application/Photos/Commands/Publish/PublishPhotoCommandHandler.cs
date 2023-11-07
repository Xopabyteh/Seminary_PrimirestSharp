using ErrorOr;
using MediatR;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services;

namespace Yearly.Application.Photos.Commands.Publish;

public class PublishPhotoCommandHandler : IRequestHandler<PublishPhotoCommand, ErrorOr<Photo>>
{
    private readonly IPhotoStorage _photoStorage;
    private readonly IPhotoRepository _photoRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public PublishPhotoCommandHandler(IPhotoStorage photoStorage, IPhotoRepository photoRepository, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _photoStorage = photoStorage;
        _photoRepository = photoRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }


    public async Task<ErrorOr<Photo>> Handle(PublishPhotoCommand request, CancellationToken cancellationToken) {

        var photoId = new PhotoId(Guid.NewGuid());
        var linkResult = await _photoStorage.UploadPhotoAsync(request.File, Photo.NameFrom(photoId, request.FoodId));

        if (linkResult.IsError)
            return linkResult.Errors;

        var photo = request.Publisher.PublishPhoto(
            photoId,
            _dateTimeProvider.UtcNow,
            request.FoodId,
            linkResult.Value);

        await _userRepository.UpdateAsync(request.Publisher);
        await _photoRepository.AddAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return photo;
    }
}
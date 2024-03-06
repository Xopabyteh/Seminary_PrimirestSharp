using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Repositories;
using Yearly.Infrastructure.Services;

namespace Yearly.Application.Photos.Commands;

public record PublishPhotoCommand(
    IFormFile File,
    FoodId FoodId,
    User Publisher) : IRequest<ErrorOr<Photo>>;

public class PublishPhotoCommandValidator : AbstractValidator<PublishPhotoCommand>
{
    public PublishPhotoCommandValidator()
    {
        RuleFor(x => x.File)
            .Must(HaveContent)
            .WithMessage("File must have content");

        RuleFor(x => x.File)
            .NotNull();

        RuleFor(x => x.FoodId)
            .NotNull();
    }

    private bool HaveContent(IFormFile arg)
    {
        return arg.Length > 0;
    }
}

public class PublishPhotoCommandHandler : IRequestHandler<PublishPhotoCommand, ErrorOr<Photo>>
{
    private readonly IPhotoStorage _photoStorage;
    private readonly IPhotoRepository _photoRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFoodRepository _foodRepository;

    public PublishPhotoCommandHandler(IPhotoStorage photoStorage, IPhotoRepository photoRepository, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IFoodRepository foodRepository)
    {
        _photoStorage = photoStorage;
        _photoRepository = photoRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
    }

    public async Task<ErrorOr<Photo>> Handle(PublishPhotoCommand request, CancellationToken cancellationToken)
    {
        //Check if the food exists
        var food = await _foodRepository.GetFoodByIdAsync(request.FoodId);
        if (food is null)
            return Errors.Errors.Food.FoodNotFound(request.FoodId);

        var photoData = await Photo.CreateImageDataFromFileAsync(
            request.File.OpenReadStream(),
            cancellationToken);

        if (photoData.IsError)
            return photoData.Errors;

        var photoId = new PhotoId(Guid.NewGuid());

        try // try-finally Scope, so that we can dispose photo data streams
        {
            //Publish photos
            var imageLinkResult = await _photoStorage.UploadPhotoAsync(
                photoData.Value.RegularPhotoData,
                Photo.NameFrom(photoId, food),
                Photo.PhotoFileExtension);

            if (imageLinkResult.IsError)
                return imageLinkResult.Errors;

            var thumbnailLinkResult = await _photoStorage.UploadPhotoAsync(
                photoData.Value.ThumbnailPhotoData,
                Photo.ThumbnailNameFrom(photoId, food),
                Photo.PhotoFileExtension);

            if (thumbnailLinkResult.IsError)
                return thumbnailLinkResult.Errors;

            var photo = request.Publisher.PublishPhoto(
                photoId,
                _dateTimeProvider.UtcNow,
                request.FoodId,
                imageLinkResult.Value,
                thumbnailLinkResult.Value);

            await _photoRepository.AddAsync(photo);
            _unitOfWork.AddForUpdate(request.Publisher);
            await _unitOfWork.SaveChangesAsync();

            return photo;
        }
        finally
        {
            await photoData.Value.RegularPhotoData.DisposeAsync();
            await photoData.Value.ThumbnailPhotoData.DisposeAsync();
        }
    }
}
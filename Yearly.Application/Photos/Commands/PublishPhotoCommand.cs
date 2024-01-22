using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
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

    private const int k_ThumbnailSize = 256;
    private const int k_MaxPhotoSize = 1024;
    private const string k_PhotoFileExtension = "jpg";
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

        using var image = await Image.LoadAsync(request.File.OpenReadStream(), cancellationToken);
        if (image.Width != image.Height)
            return Error.Validation("Photo have be 1 / 1 aspect ratio"); //Todo:

        if (image.Width < k_ThumbnailSize)
            return Error.Validation($"Photo have to be at least {k_ThumbnailSize} pixels in side length"); //Todo:

        //Resize to k_MaxPhotoSize if necessary
        if (image.Width > k_MaxPhotoSize)
        {
            image.Mutate(i =>
            {
                i.Resize(k_MaxPhotoSize, k_MaxPhotoSize);
            });
        }

        //Create thumbnail
        var thumbnailImage = image.Clone(i =>
        {
            i.Resize(k_ThumbnailSize, k_ThumbnailSize);
        });

        //Save to streams encoded as Jpeg
        var encoder = new JpegEncoder()
        {
            Quality = 80
        };
        using var imageOutputStream = new MemoryStream();
        using var thumbnailOutputStream = new MemoryStream();
        var saveTasks = new Task[]
        {
            image.SaveAsync(imageOutputStream, encoder, cancellationToken),
            thumbnailImage.SaveAsync(thumbnailOutputStream, encoder, cancellationToken)
        };
        await Task.WhenAll(saveTasks);
        
        //Move stream positions to 0 so they can be read
        imageOutputStream.Position = 0;
        thumbnailOutputStream.Position = 0;

        var photoId = new PhotoId(Guid.NewGuid());

        var imageLinkResult = await _photoStorage.UploadPhotoAsync(
            imageOutputStream,
            Photo.NameFrom(photoId, food),
            k_PhotoFileExtension);

        if (imageLinkResult.IsError)
            return imageLinkResult.Errors;

        var thumbnailLinkResult = await _photoStorage.UploadPhotoAsync(
            thumbnailOutputStream,
            Photo.ThumbnailNameFrom(photoId, food),
            k_PhotoFileExtension);

        if(thumbnailLinkResult.IsError)
            return thumbnailLinkResult.Errors;

        var photo = request.Publisher.PublishPhoto(
            photoId,
            _dateTimeProvider.UtcNow,
            request.FoodId,
            imageLinkResult.Value,
            thumbnailLinkResult.Value);

        await _photoRepository.AddAsync(photo);
        await _unitOfWork.SaveChangesAsync();

        return photo;
    }
}
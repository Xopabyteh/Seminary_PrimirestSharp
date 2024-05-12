using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Repositories;

namespace Yearly.Application.Photos.DomainEvents;

public class CreatePhotoThumbnailOnUserPublishedPhoto : INotificationHandler<UserPublishedNewPhotoDomainEvent>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoStorage _photoStorage;
    private readonly IOptions<PhotoOptions> _photoOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePhotoThumbnailOnUserPublishedPhoto> _logger;

    public CreatePhotoThumbnailOnUserPublishedPhoto(
        IPhotoRepository photoRepository,
        IHttpClientFactory httpClientFactory,
        IOptions<PhotoOptions> photoOptions,
        IPhotoStorage photoStorage,
        IUnitOfWork unitOfWork,
        ILogger<CreatePhotoThumbnailOnUserPublishedPhoto> logger)
    {
        _photoRepository = photoRepository;
        _httpClientFactory = httpClientFactory;
        _photoOptions = photoOptions;
        _photoStorage = photoStorage;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(UserPublishedNewPhotoDomainEvent request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetAsync(request.PhotoId);
        if (photo is null)
        {
            // Photo could've been deleted
            // Ignore it
            return;
        }

        // Download original photo from resource link
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(photo.ResourceLink, cancellationToken);
        response.EnsureSuccessStatusCode();

        // Create thumbnail
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var thumbnailData = await Photo.CreateThumbnailFromImageAsync(
            stream,
            _photoOptions.Value,
            cancellationToken);

        // Upload thumbnail to storage
        var imageLink = await _photoStorage.UploadPhotoAsync(
            thumbnailData,
            Photo.ThumbnailNameFrom(photo.Id),
            Photo.PhotoFileExtension, 
            cancellationToken);

        // Set & Save
        var result = photo.SetThumbnail(imageLink);
        if (result.IsError)
        {
            // Log error and ignore
            _logger.LogError("Error setting thumbnail for photo {PhotoId}: {Error}", photo.Id, result.Errors);
            return;
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
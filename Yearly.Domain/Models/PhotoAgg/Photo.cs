using ErrorOr;
using MediatR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.DomainEvents;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.PhotoAgg;

public class Photo : AggregateRoot<PhotoId>
{
    public const string PhotoFileExtension = "jpg";

    public static string NameFrom(PhotoId photoId, Food food)
        => $"{food.Name}-{photoId.Value}";
    public static string ThumbnailNameFrom(PhotoId photoId)
        => $"{photoId.Value}-thumbnail";

    public UserId PublisherId { get; private set; }
    public DateTime PublishDate { get; private set; }
    public FoodId FoodId { get; private set; }
    public string ResourceLink { get; private set; }

    /// <summary>
    /// If null, no thumbnail exists yet
    /// </summary>
    public string? ThumbnailResourceLink { get; private set; }
 
    public bool IsApproved { get; private set; }

    internal Photo(
        PhotoId id,
        UserId publisherId,
        DateTime publishDate,
        FoodId foodId,
        string resourceLink) 
        : base(id)
    {
        PublisherId = publisherId;
        PublishDate = publishDate;
        FoodId = foodId;
        ResourceLink = resourceLink;

        IsApproved = false;
    }

    public ErrorOr<Unit> SetThumbnail(string thumbnailResourceLink)
    {
        if (this.ThumbnailResourceLink is not null)
            return Errors.Errors.Photo.ThumbnailAlreadySet();

        ThumbnailResourceLink = thumbnailResourceLink;
        PublishDomainEvent(new PhotoThumbnailWasSet(this.Id));

        return Unit.Value;
    }


    internal void Approve(UserId approverId)
    {
        if (this.IsApproved)
            throw new IllegalStateException("Photo already approved");

        this.IsApproved = true;
        
        // Domain event published here to reach the change tracker
        PublishDomainEvent(new UserApprovedPhotoDomainEvent(approverId, this.Id)); 
    }


#pragma warning disable CS8618 //For EF Core
    // ReSharper disable once UnusedMember.Local
    private Photo(string thumbnailResourceLink)
        : base(null!)
#pragma warning restore CS8618
    {
        ThumbnailResourceLink = thumbnailResourceLink;
    }

    public static async Task<ErrorOr<MemoryStream>> CreateImageFromFileDataAsync(
        Stream imageFileData,
        PhotoOptions options,
        CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(imageFileData, cancellationToken);

        if (image.Width < options.ThumbnailSize)
            return Errors.Errors.Photo.TooSmall(options.ThumbnailSize);

        // Resize photo to cap bigger side to MaxSideLength
        if (image.Width > options.MaxSideLength)
        {
            image.Mutate(i =>
            {
                i.Resize(options.MaxSideLength, 0);
            });
        } 
        else if (image.Height > options.MaxSideLength)
        {
            image.Mutate(i =>
            {
                i.Resize(0, options.MaxSideLength);
            });
        }

        // Save to stream encoded as Jpeg
        var encoder = new JpegEncoder()
        {
            Quality = options.PhotoJpegQuality
        };

        var imageOutputStream = new MemoryStream();
        await image.SaveAsync(imageOutputStream, encoder, cancellationToken);

        // Move stream position to 0 so it can be read
        imageOutputStream.Position = 0;

        return imageOutputStream;
    }

    public static async Task<MemoryStream> CreateThumbnailFromImageAsync(
        Stream imageFileData,
        PhotoOptions options,
        CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(imageFileData, cancellationToken);

        if (image.Width != image.Height)
        {
            // Crop from sides
            var minDimension = Math.Min(image.Width, image.Height);
            var cropRectangle = new Rectangle(
                (image.Width - minDimension) / 2,
                (image.Height - minDimension) / 2,
                minDimension,
                minDimension);

            image.Mutate(x => x.Crop(cropRectangle));
        }

        // Resize photo to thumbnail size
        if (image.Width > options.ThumbnailSize)
        {
            image.Mutate(i =>
            {
                i.Resize(options.ThumbnailSize, options.ThumbnailSize);
            });
        }

        // Save to stream encoded as Jpeg
        var encoder = new JpegEncoder()
        {
            Quality = options.ThumbnailJpegQuality
        };

        var imageOutputStream = new MemoryStream();
        await image.SaveAsync(imageOutputStream, encoder, cancellationToken);

        // Move stream position to 0 so it can be read
        imageOutputStream.Position = 0;

        return imageOutputStream;
    }
}
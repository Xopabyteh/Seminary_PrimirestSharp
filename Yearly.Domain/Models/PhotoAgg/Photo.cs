using ErrorOr;
using MediatR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Threading;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.PhotoAgg;

public class Photo : AggregateRoot<PhotoId>
{
    public const string PhotoFileExtension = "jpg";

    public static string NameFrom(PhotoId photoId, Food food)
        => $"{food.Name}-{photoId.Value}";
    public static string ThumbnailNameFrom(PhotoId photoId, Food food)
        => $"{food.Name}-{photoId.Value}-t";
    public UserId PublisherId { get; private set; }
    public DateTime PublishDate { get; private set; }
    public FoodId FoodId { get; private set; }
    public string ResourceLink { get; private set; }
    public string ThumbnailResourceLink { get; private set; }
    public bool IsApproved { get; private set; }

    internal Photo(
        PhotoId id,
        UserId publisherId,
        DateTime publishDate,
        FoodId foodId,
        string resourceLink,
        string thumbnailResourceLink) 
        : base(id)
    {
        PublisherId = publisherId;
        PublishDate = publishDate;
        FoodId = foodId;
        ResourceLink = resourceLink;
        ThumbnailResourceLink = thumbnailResourceLink;

        IsApproved = false;
    }

    internal void Approve()
    {
        if (this.IsApproved)
            throw new IllegalStateException("Photo already approved");

        this.IsApproved = true;
    }

#pragma warning disable CS8618 //For EF Core
    private Photo(string thumbnailResourceLink)
        : base(null!)
#pragma warning restore CS8618
    {
        ThumbnailResourceLink = thumbnailResourceLink;
    }

    /// <summary>
    /// Creates memory stream data of processed images from the given file image data.
    /// The memory streams are filled and with position = 0 so they can be read.
    /// Make sure to dispose the streams.
    /// </summary>
    /// <param name="fileData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<ErrorOr<(MemoryStream RegularPhotoData, MemoryStream ThumbnailPhotoData)>> CreatePhotosFromFileAsync(
        Stream fileData,
        CancellationToken cancellationToken)
    {
        const int thumbnailSize = 256;
        const int maxPhotoSize = 1024;

        using var image = await Image.LoadAsync(fileData, cancellationToken);
        if (image.Width != image.Height)
            return Errors.Errors.Photo.IncorrectAspectRatio;

        if (image.Width < thumbnailSize)
            return Errors.Errors.Photo.TooSmall(thumbnailSize);

        //Resize to k_MaxPhotoSize if necessary
        if (image.Width > maxPhotoSize)
        {
            image.Mutate(i =>
            {
                i.Resize(maxPhotoSize, maxPhotoSize);
            });
        }

        //Create thumbnail
        var thumbnailImage = image.Clone(i =>
        {
            i.Resize(thumbnailSize, thumbnailSize);
        });

        //Save to streams encoded as Jpeg
        var encoder = new JpegEncoder()
        {
            Quality = 80
        };

        var imageOutputStream = new MemoryStream();
        var thumbnailOutputStream = new MemoryStream();
        var saveTasks = new Task[]
        {
            image.SaveAsync(imageOutputStream, encoder, cancellationToken),
            thumbnailImage.SaveAsync(thumbnailOutputStream, encoder, cancellationToken)
        };
        await Task.WhenAll(saveTasks);

        //Move stream positions to 0 so they can be read
        imageOutputStream.Position = 0;
        thumbnailOutputStream.Position = 0;


        return (imageOutputStream, thumbnailOutputStream);
    }
}
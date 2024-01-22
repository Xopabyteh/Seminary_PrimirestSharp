using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.PhotoAgg;

public class Photo : AggregateRoot<PhotoId>
{
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
}
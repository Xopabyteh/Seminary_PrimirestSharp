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

    public UserId PublisherId { get; private set; }
    public DateTime PublishDate { get; private set; }
    public FoodId FoodId { get; private set; }
    public string Link { get; private set; }
    public bool IsApproved { get; private set; }

    public Photo(PhotoId id, UserId publisherId, DateTime publishDate, FoodId foodId, string link) 
        : base(id)
    {
        PublisherId = publisherId;
        PublishDate = publishDate;
        FoodId = foodId;
        Link = link;

        IsApproved = false;
    }

    public void Approve()
    {
        if (this.IsApproved)
            throw new IllegalStateException("Photo already approved");

        this.IsApproved = true;
    }

#pragma warning disable CS8618 //For EF Core
    private Photo()
        : base(null!)
#pragma warning restore CS8618
    {

    }
}
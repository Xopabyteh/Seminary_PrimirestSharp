using Yearly.Domain.Errors.Exceptions;

namespace Yearly.Domain.Models.PhotoAgg;

public class Photo : AggregateRoot<Guid>
{
    public static string NameFrom(Guid photoId, Guid foodId)
        => $"{foodId}-{photoId}";

    public int PublisherId { get; private set; }
    public DateTime PublishDate { get; private set; }
    public Guid FoodId { get; private set; }
    public string Link { get; private set; }
    public bool IsApproved { get; private set; }

    public Photo(Guid id, int publisherId, DateTime publishDate, Guid foodId, string link) 
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
#pragma warning restore CS8618
    {

    }
}
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg;

public class User : AggregateRoot<UserId, int>
{
    public string Username { get; private set; }

    private readonly List<UserRole> _roles;
    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Published photos by this user
    /// </summary>
    private readonly List<PhotoId> _photoIds;
    public IReadOnlyList<PhotoId> PhotoIds => _photoIds.AsReadOnly();

    public User(UserId id, string username)
        : base(id)
    {
        Username = username;
        _roles = new List<UserRole>();
        _photoIds = new List<PhotoId>();
    }

    public void AddRole(UserRole role)
    {
        _roles.Add(role);
    }

    public void ApprovePhoto(Photo photo)
    {
        photo.Approve();

        // Publish Domain events
    }

    public void RejectPhoto(Photo photo)
    {
        if(photo.IsApproved)
            throw new IllegalStateException("Cannot reject an approved photo");

        // Publish Domain events
    }

    public Photo PublishPhoto(PhotoId photoId, DateTime publishDate, FoodId forFoodId, string photoLink)
    {
        var photo = new Photo(
            photoId,
            new UserId(this.Id.Value),
            publishDate,
            forFoodId,
            photoLink);

        _photoIds.Add(photoId);

        // Publish Domain events

        //Automatically approve photo if user is a photo verifier
        if(this.Roles.Contains(UserRole.PhotoApprover))
            photo.Approve();

        return photo;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(null!)
    {
        
    }
}
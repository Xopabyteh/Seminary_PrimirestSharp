using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg;

public class User : AggregateRoot<int>
{
    public string Username { get; private set; }

    private readonly List<UserRole> _roles;
    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Published photos by this user
    /// </summary>
    //private readonly List<Guid> _photoIds;
    //public IReadOnlyList<Guid> PhotoIds => _photoIds.AsReadOnly();

    private readonly List<Photo> _photos;
    public IReadOnlyList<Photo> Photos => _photos.AsReadOnly();

    public User(int id, string username)
        : base(id)
    {
        Username = username;
        _roles = new List<UserRole>();
        _photos = new List<Photo>();
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

    public Photo PublishPhoto(Guid photoId, DateTime publishDate, Food forFood, string photoLink)
    {
        var photo = new Photo(
            photoId,
            this,
            publishDate,
            forFood,
            photoLink);

        _photos.Add(photo);

        // Publish Domain events

        //Automatically approve photo if user is a photo verifier
        if (this.Roles.Contains(UserRole.PhotoApprover))
        {
            photo.Approve();
        }

        return photo;
    }
}
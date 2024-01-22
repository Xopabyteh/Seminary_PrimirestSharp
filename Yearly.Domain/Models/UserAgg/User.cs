
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg;

public class User : AggregateRoot<UserId>, IDomainEventPublisher
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
        PublishDomainEvent(new RoleAddedToUserDomainEvent(this.Id));
    }

    public void RemoveRole(UserRole role)
    {
        _roles.Remove(role);
        PublishDomainEvent(new RoleRemovedFromUserDomainEvent(this.Id));
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

    public Photo PublishPhoto(
        PhotoId photoId,
        DateTime publishDate,
        FoodId forFoodId,
        string photoResourceLink,
        string photoThumbnailResourceLink)
    {
        var photo = new Photo(
            photoId,
            this.Id,
            publishDate,
            forFoodId,
            photoResourceLink,
            photoThumbnailResourceLink);

        _photoIds.Add(photoId);

        //Automatically approve photo if user is a photo verifier
        if (this.Roles.Contains(UserRole.PhotoApprover))
        {
            photo.Approve();
        }

        PublishDomainEvent(new UserPublishedNewPhotoDomainEvent(this.Id, photo.Id));

        return photo;
    }
    public void ClearDomainEvents()
        =>_publishedEvents.Clear();

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
        => _publishedEvents.ToList().AsReadOnly();

    private readonly List<IDomainEvent> _publishedEvents = new();
    private void PublishDomainEvent(IDomainEvent dEvent)
        => _publishedEvents.Add(dEvent);
}
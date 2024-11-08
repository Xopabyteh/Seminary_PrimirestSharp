using System.Text.Json.Serialization;
using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.Common.ValueObjects;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.DomainEvents;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Models.UserAgg;

public class User : AggregateRoot<UserId>
{
    public string Username { get; private set; }

    private readonly List<UserRole> _roles;
    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Published photos by this user
    /// </summary>
    private readonly List<PhotoId> _photoIds;
    public IReadOnlyList<PhotoId> PhotoIds => _photoIds.AsReadOnly();

    public UserPricingGroup PricingGroup { get; private set; }

    public User(UserId id, string username, UserPricingGroup pricingGroup)
        : base(id)
    {
        Username = username;
        _roles = new List<UserRole>();
        _photoIds = new List<PhotoId>();
        PricingGroup = pricingGroup;
    }

    // For EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }

    public Photo PublishPhoto(
        PhotoId photoId,
        DateTime publishDate,
        FoodId forFoodId,
        string photoResourceLink)
    {
        var photo = new Photo(
            photoId,
            this.Id,
            publishDate,
            forFoodId,
            photoResourceLink);

        _photoIds.Add(photoId);

        PublishDomainEvent(new UserPublishedNewPhotoDomainEvent(this.Id, photo.Id));

        //Automatically approve photo if user is a photo verifier
        if (this.Roles.Contains(UserRole.PhotoApprover))
        {
            this.ApprovePhoto(photo);
        }

        return photo;
    }

    public void UpdatePricingGroup(UserPricingGroup pricingGroup)
    {
        PricingGroup = pricingGroup;
    }

    internal void ApprovePhoto(Photo photo)
    {
        photo.Approve(approverId: this.Id);
    }

    internal void RejectPhoto(Photo photo)
    {
        if (photo.IsApproved)
            throw new RejectingApprovedPhotoException(this, photo);
    }
    internal void AddRole(UserRole role, User toUser)
    {
        toUser.AddRole(role);
    }
    internal void RemoveRole(UserRole role, User fromUser)
    {
        fromUser.RemoveRole(role);
    }

    internal void UpdateRoles(User ofUser, List<UserRole> roles)
    {
        ofUser.UpdateRoles(roles);
    }
    private void AddRole(UserRole role)
    {
        _roles.Add(role);
    }

    private void RemoveRole(UserRole role)
    {
        _roles.Remove(role);
    }

    private void UpdateRoles(List<UserRole> roles)
    {
        _roles.Clear();
        _roles.AddRange(roles);
    }
}

public class PhotoApprover
{
    private readonly User user;

    public PhotoApprover(User user)
    {
        this.user = user;
    }

    public static PhotoApprover FromUser(User user)
    {
        return new PhotoApprover(user);
    }

    public void ApprovePhoto(Photo photo)
    {
        user.ApprovePhoto(photo);
    }

    public void RejectPhoto(Photo photo)
    {
        user.RejectPhoto(photo);
    }
}

public class Admin
{
    private readonly User user;

    public Admin(User user)
    {
        this.user = user;
    }

    public static Admin FromUser(User user)
    {
        return new Admin(user);
    }

    public void AddRole(UserRole role, User toUser)
    {
        user.AddRole(role, toUser);
    }

    public void RemoveRole(UserRole role, User fromUser)
    {
        user.RemoveRole(role, fromUser);
    }

    public void UpdateRoles(User ofUser, List<UserRole> roles)
    {
        ofUser.UpdateRoles(ofUser, roles);
    }
}
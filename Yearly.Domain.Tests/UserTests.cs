using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Tests;
public class UserTests
{
    [Fact]
    public void User_GetsRoleWhenAdminAddsIt()
    {
        // Arrange
        var user = new User(new UserId(0), "some user");
        var adminUser = new User(new UserId(1), "admin");
        var admin = Admin.FromUser(adminUser);
        var roleToAdd = UserRole.PhotoApprover;

        // Act
        admin.AddRole(roleToAdd, user);

        // Assert
        Assert.Contains(roleToAdd, user.Roles);
    }

    [Fact]
    public void User_RoleGetsRemovedWhenAdminRemovesIt()
    {
        // Arrange
        var user = new User(new UserId(0), "some user");
        var adminUser = new User(new UserId(1), "admin");
        var admin = Admin.FromUser(adminUser);
        var roleToRemove = UserRole.PhotoApprover;
        admin.AddRole(roleToRemove, user);

        // Act
        admin.RemoveRole(roleToRemove, user);

        // Assert
        Assert.DoesNotContain(roleToRemove, user.Roles);
    }

    [Fact]
    public void User_OwnsPhotoWhenHePublishesIt()
    {
        // Arrange
        var publisher = new User(new UserId(0), "some publisher");
        var someFoodId = new FoodId(Guid.NewGuid());
        var someResourceLink = "some resource link";

        //Act
        var photo = publisher.PublishPhoto(new PhotoId(Guid.NewGuid()), DateTime.UtcNow, someFoodId, someResourceLink);

        // Assert
        Assert.Contains(photo.Id, publisher.PhotoIds);
    }

    [Fact]
    public void User_ApproverCannotRejectApprovedPhoto()
    {
        // Arrange
        var publisher = new User(new UserId(0), "some publisher");
        var photo = publisher.PublishPhoto(
            new PhotoId(Guid.NewGuid()),
            DateTime.UtcNow,
            new FoodId(Guid.NewGuid()),
                       "some resource link");

        var approver = PhotoApprover.FromUser(new User(new UserId(1), "some approver"));
        approver.ApprovePhoto(photo);

        // Act
        var exception = Record.Exception(() => approver.RejectPhoto(photo));

        // Assert
        Assert.IsType<RejectingApprovedPhotoException>(exception);
    }
}
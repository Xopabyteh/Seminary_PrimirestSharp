using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Domain.Tests;

public class PhotoTests
{
    [Fact]
    public void Photo_IsApprovedWhenPhotoApproverApprovesIt()
    {
        // Arrange
        var publisher = new User(new UserId(0), "some publisher");
        var photo = publisher.PublishPhoto(new PhotoId(
                Guid.NewGuid()),
                DateTime.UtcNow,
                new FoodId(Guid.NewGuid()),
                "some resource link",
                "some thumbnail resource link");

        var someApproverUser = new User(new UserId(1), "some approver");
        var someApprover = PhotoApprover.FromUser(someApproverUser);

        // Act
        someApprover.ApprovePhoto(photo);

        // Assert
        Assert.True(photo.IsApproved);
    }

    [Fact]
    public void Photo_IsAutomaticallyApprovedWhenPublishedByPhotoApprover()
    {
        // Arrange
        var someApproverPublisher = new User(new UserId(0), "some approver publisher");
        var someAdmin = Admin.FromUser(new User(new UserId(1), "some admin"));
        someAdmin.AddRole(UserRole.PhotoApprover, someApproverPublisher);

        // Act
        var photo = someApproverPublisher.PublishPhoto(new PhotoId(
                Guid.NewGuid()),
                DateTime.UtcNow,
                new FoodId(Guid.NewGuid()),
                "some resource link",
                "some thumbnail resource link");

        // Assert
        Assert.True(photo.IsApproved);
    }
}
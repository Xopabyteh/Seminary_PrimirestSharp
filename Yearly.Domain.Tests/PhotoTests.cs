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
        var publisher = new User(new UserId(0), "some publisher", UserPricingGroup.MoreOrEqual15YearsOldStudent);
        var photo = publisher.PublishPhoto(new PhotoId(
                Guid.NewGuid()),
                DateTime.UtcNow,
                new FoodId(Guid.NewGuid()),
                "some resource link");

        var someApproverUser = new User(new UserId(1), "some approver", UserPricingGroup.MoreOrEqual15YearsOldStudent);
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
        var someApproverPublisher = new User(new UserId(0), "some approver publisher", UserPricingGroup.MoreOrEqual15YearsOldStudent);
        var someAdmin = Admin.FromUser(new User(new UserId(1), "some admin", UserPricingGroup.MoreOrEqual15YearsOldStudent));
        someAdmin.AddRole(UserRole.PhotoApprover, someApproverPublisher);

        // Act
        var photo = someApproverPublisher.PublishPhoto(new PhotoId(
                Guid.NewGuid()),
                DateTime.UtcNow,
                new FoodId(Guid.NewGuid()),
                "some resource link");

        // Assert
        Assert.True(photo.IsApproved);
    }
}
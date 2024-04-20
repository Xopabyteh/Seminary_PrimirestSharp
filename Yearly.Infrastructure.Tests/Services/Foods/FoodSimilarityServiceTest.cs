using Microsoft.Extensions.Options;
using Moq;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Infrastructure.Services.Foods;

namespace Yearly.Infrastructure.Tests.Services.Foods;

public class FoodSimilarityServiceTest
{
    [Fact]
    public void Service_CreatesSimilarityRecords_ComparedAgainstEachOther()
    {
        // Arrange
        var iOptionsMock = new Mock<IOptions<FoodSimilarityServiceOptions>>();
        iOptionsMock.Setup(o => o.Value).Returns(new FoodSimilarityServiceOptions()
        {
            NameStringSimilarityThreshold = 0.8d
        });

        var iUnitOfWorkMock = new Mock<IUnitOfWork>();

        var firstFoodId = new FoodId(Guid.NewGuid());
        var similarFoodFoodId = new FoodId(Guid.NewGuid());
        var newlyLearnedFoodsViews = new List<FoodSimilarityService.FoodView>()
        {
            new(firstFoodId, "Kuřecí stehno s rýží"),
            new(similarFoodFoodId, "Kuřecí stehno, s rýží"),
            new(new FoodId(Guid.NewGuid()), "Jiné jídlo"),
        };
        var foodsInDbViews = new List<FoodSimilarityService.FoodView>()
        {
            new(new FoodId(Guid.NewGuid()), "Kuřecí stehno s rýží, ze včerejška"),
        };
        var sut = new FoodSimilarityService(null!, iOptionsMock.Object, iUnitOfWorkMock.Object);

        // Act
        var result = sut.CreateFoodSimilarityRecords(newlyLearnedFoodsViews, foodsInDbViews);

        // Assert
        var didWeMatchObviousSimilarity = result.Any(s =>
            (s.NewlyPersistedFoodId == similarFoodFoodId && s.PotentialAliasOriginId == firstFoodId)
            || (s.NewlyPersistedFoodId == firstFoodId && s.PotentialAliasOriginId == similarFoodFoodId));
        
        Assert.True(didWeMatchObviousSimilarity);
    }

    [Fact]
    public void Service_CreatesSimilarityRecords_ComparedAgainstDb()
    {
        // Arrange
        var iOptionsMock = new Mock<IOptions<FoodSimilarityServiceOptions>>();
        iOptionsMock.Setup(o => o.Value).Returns(new FoodSimilarityServiceOptions()
        {
            NameStringSimilarityThreshold = 0.8d
        });
        
        var iUnitOfWorkMock = new Mock<IUnitOfWork>();

        var newFoodId = new FoodId(Guid.NewGuid());
        var alreadyLearnedFoodId = new FoodId(Guid.NewGuid());
        var newlyLearnedFoodsViews = new List<FoodSimilarityService.FoodView>()
        {
            new(newFoodId, "Kuřecí stehno s rýží"),
        };
        var foodsInDbViews = new List<FoodSimilarityService.FoodView>()
        {
            new(alreadyLearnedFoodId, "Kuřecí stehno, s rýží"),
        };
        var sut = new FoodSimilarityService(null!, iOptionsMock.Object, iUnitOfWorkMock.Object);

        // Act
        var result = sut.CreateFoodSimilarityRecords(newlyLearnedFoodsViews, foodsInDbViews);

        // Assert
        var didCreateRecordWithAliasForAlready = result.Any(s =>
            s.NewlyPersistedFoodId == newFoodId && s.PotentialAliasOriginId == alreadyLearnedFoodId);
        Assert.True(didCreateRecordWithAliasForAlready);
    }
}
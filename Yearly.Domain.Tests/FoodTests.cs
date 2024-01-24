using Yearly.Domain.Errors.Exceptions;
using Yearly.Domain.Models.FoodAgg;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Domain.Tests;

public class FoodTests
{
    [Fact]
    public void Food_SetsAliasToAnotherFood()
    {
        // Arrange
        var food = Food.Create(new FoodId(Guid.NewGuid()), "some food", "", new PrimirestFoodIdentifier(1, 2, 3));
        var potentialAliasOriginFood = Food.Create(new FoodId(Guid.NewGuid()), "some similar food", "", new PrimirestFoodIdentifier(4, 5, 6));

        // Act
        food.SetAliasForFood(potentialAliasOriginFood);

        // Assert
        Assert.Equal(potentialAliasOriginFood.Id, food.AliasForFoodId);
    }

    [Fact]
    public void Food_CannotSetAliasOfFoodForFoodWithAnAlias()
    {
        // Arrange
        var food = Food.Create(new FoodId(Guid.NewGuid()), "some food", "", new PrimirestFoodIdentifier(1, 2, 3));
        var theActualAliasOriginFood = Food.Create(new FoodId(Guid.NewGuid()), "some similar food 1", "", new PrimirestFoodIdentifier(4, 5, 6));
        var potentialAliasOriginFood = Food.Create(new FoodId(Guid.NewGuid()), "some similar food 2", "", new PrimirestFoodIdentifier(7, 8, 9));
        potentialAliasOriginFood.SetAliasForFood(theActualAliasOriginFood);

        // Act
        var exception = Record.Exception(() => food.SetAliasForFood(potentialAliasOriginFood));

        // Assert
        Assert.IsType<SetAliasToFoodWithAliasException>(exception);
    }
}
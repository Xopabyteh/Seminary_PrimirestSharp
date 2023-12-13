using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Menus;

/// <param name="FoodItemId">The id of the food ordered, that can be retrieved from our database using the <see cref="PrimirestFoodIdentifier.ItemId"/></param>
public readonly record struct PrimirestFoodOrderIdentifier(
    // These three are used when canceling an order of the item
    int OrderItemId, // This is the ID of the order item : ID
    int OrderId, // This is the ID of the order          : IDOrder
    int MenuId, // This is the ID of the menu            : IDItem

    int FoodItemId // THIS POINTS TO THE FOOD ID OF THE PRIMIREST FOOD IDENTIFIER
    );

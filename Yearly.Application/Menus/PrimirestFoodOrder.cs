using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Menus;

/// <summary>
/// 
/// </summary>
/// <param name="OrderItemId"></param>
/// <param name="OrderId"></param>
/// <param name="FoodItemId">The id of the food ordered, that can be retrieved from our database using the <see cref="PrimirestFoodIdentifier.ItemId"/></param>
public readonly record struct PrimirestFoodOrder(
    // These two are used when canceling an order of the item
    int OrderItemId, // This is the ID of the order item
    int OrderId, // This is the ID of the order

    int FoodItemId // THIS POINTS TO THE FOOD ID OF THE PRIMIREST FOOD IDENTIFIER
    );

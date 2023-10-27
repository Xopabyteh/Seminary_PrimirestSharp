namespace Yearly.Application.Menus;

public readonly record struct PrimirestFoodOrder(
    int OrderId, // This is the ID of the order
    int ItemId, // This is the ID of the order item
    int FoodId // THIS POINTS TO THE FOOD ID OF THE PRIMIREST FOOD IDENTIFIER
    );

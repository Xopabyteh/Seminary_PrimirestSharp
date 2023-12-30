using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Orders;

public readonly record struct PrimirestFood(
    string Name,
    string Allergens,
    PrimirestFoodIdentifier PrimirestFoodIdentifier);
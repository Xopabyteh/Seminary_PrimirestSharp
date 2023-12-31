using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Orders.PrimirestStructures;

public readonly record struct PrimirestFood(
    string Name,
    string Allergens,
    PrimirestFoodIdentifier PrimirestFoodIdentifier);
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Menus;

public readonly record struct PrimirestFood(
    string Name,
    string Allergens,
    PrimirestOrderIdentifier PrimirestOrderIdentifier);
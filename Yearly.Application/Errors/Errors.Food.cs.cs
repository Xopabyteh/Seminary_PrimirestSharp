using ErrorOr;
using Yearly.Domain.Models.FoodAgg.ValueObjects;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Food
    {
        public static Error FoodNotFound(FoodId foodId)
            => Error.NotFound("Food.FoodNotFound", $"Food with the id {foodId.Value} was not found");
    }
}
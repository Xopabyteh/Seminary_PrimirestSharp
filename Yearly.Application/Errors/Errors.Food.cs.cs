using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Food
    {
        public static Error FoodNotFound
            => Error.NotFound("Food.FoodNotFound", "Food not found");
    }
}
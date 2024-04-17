using ErrorOr;
using Yearly.Domain.Models.Common.ValueObjects;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Orders
    {
        public static Error InsufficientFunds
            => Error.Validation("Orders.InsufficientFunds", "You have insufficient funds");

        public static Error TooLateToOrder
            => Error.Validation("Orders.TooLateToOrder", "It is too late to order food");

        public static Error TooLateToCancelOrder
            => Error.Validation("Orders.TooLateToCancelOrder", "It is too late to cancel the order");

        public static Error AlreadyConsumed
            => Error.Validation("Orders.AlreadyConsumed","You ate this already");

        public static Error InvalidFoodIdentifier
            => Error.Validation("Orders.InvalidFoodIdentifier", "The food identifier does not exist");

        public static Error InvalidOrderIdentifier
            => Error.Validation("Orders.InvalidOrderIdentifier", "The order identifier does not exist");

        public static Error InvalidWeeklyMenuId
            => Error.Validation("Orders.InvalidWeeklyMenuId", "The weekly menu id does not exist");
    }
}
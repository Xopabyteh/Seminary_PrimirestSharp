using ErrorOr;
using Yearly.Contracts;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Orders
    {
        public static Error InsufficientFunds
            => Error.Validation(ErrorCodes.Orders.InsufficientFunds, "You have insufficient funds");

        public static Error TooLateToOrder
            => Error.Validation(ErrorCodes.Orders.TooLateToOrder, "It is too late to order food");

        public static Error TooLateToCancelOrder
            => Error.Validation(ErrorCodes.Orders.TooLateToCancelOrder, "It is too late to cancel the order");

        public static Error AlreadyConsumed
            => Error.Validation(ErrorCodes.Orders.AlreadyConsumed,"You ate this already");

        public static Error InvalidFoodIdentifier
            => Error.Validation(ErrorCodes.Orders.InvalidFoodIdentifier, "The food identifier does not exist");

        public static Error InvalidOrderIdentifier
            => Error.Validation(ErrorCodes.Orders.InvalidOrderIdentifier, "The order identifier does not exist");

        public static Error InvalidWeeklyMenuId
            => Error.Validation(ErrorCodes.Orders.InvalidWeeklyMenuId, "The weekly menu id does not exist");
    }
}
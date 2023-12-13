using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Orders
    {
        public static readonly Error InsufficientFunds
            = Error.Validation("Orders.InsufficientFunds", "You have insufficient funds");

        public static readonly Error TooLateToOrder
            = Error.Validation("Orders.TooLateToOrder", "It is too late to order food");

        public static readonly Error TooLateToCancelOrder
            = Error.Validation("Orders.TooLateToCancelOrder", "It is too late to cancel the order");
    }
}
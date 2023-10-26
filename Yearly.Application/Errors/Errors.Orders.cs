using ErrorOr;

namespace Yearly.Application.Errors;

public static partial class Errors
{
    public static class Orders
    {
        public static Error InsufficientFunds
            => Error.Failure("Orders.InsufficientFunds", "You have insufficient funds");

        public static Error TooLateToOrder
            => Error.Failure("Orders.TooLateToOrder", "It is too late to order food");
    }
}
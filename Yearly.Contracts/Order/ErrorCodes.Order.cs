namespace Yearly.Contracts;

public static partial class ErrorCodes
{
    public static class Orders
    {
        public const string InsufficientFunds = "Orders.InsufficientFunds";
        public const string TooLateToOrder = "Orders.TooLateToOrder";
        public const string TooLateToCancelOrder = "Orders.TooLateToCancelOrder";
        public const string AlreadyConsumed = "Orders.AlreadyConsumed";
        public const string InvalidFoodIdentifier = "Orders.InvalidFoodIdentifier";
        public const string InvalidOrderIdentifier = "Orders.InvalidOrderIdentifier";
        public const string InvalidWeeklyMenuId = "Orders.InvalidWeeklyMenuId";
    }
}
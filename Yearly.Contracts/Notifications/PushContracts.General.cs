namespace Yearly.Contracts.Notifications;

public static partial class PushContracts
{
    public static class General
    {
        public const string k_NotificationIdKey = "NotificationId"; //A notification shall always have this assigned

        /// <summary>
        /// General for android.
        /// </summary>
        public const string k_GeneralNotificationChannelId = "General";
    }
}
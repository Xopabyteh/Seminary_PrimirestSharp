namespace Yearly.Contracts.Notifications;
public static partial class PushContracts
{
    public class Photos
    {
        public const int k_PhotoApprovedNotificationId = 101;
        public static string CreatePhotoApprovedTopic(int userId)
            => CreateTopic("PhotoApproved", k_PhotoApprovedNotificationId, userId);

        
        public const int k_NewWaitingPhotoNotificationId = 102;
        public static string NewWaitingPhotoTopic = CreateTopic("NewWaitingPhoto", k_NewWaitingPhotoNotificationId);
    }
}

namespace Yearly.Contracts.Notifications;

public class NotificationTagContract
{
    public string Value { get; init; }
    public int NotificationId { get; init; } // >= 1!!!
    public bool IsUserSpecific { get; init; }

    private static NotificationTagContract CreateGeneralTag(string value, int notificationId)
    {
        return new NotificationTagContract(value, notificationId, false);
    }

    private static NotificationTagContract CreateUserSpecificTag(string value, int notificationId, int userId)
    {
        return new NotificationTagContract($"{value}_{userId}", notificationId, true);
    }

    private NotificationTagContract(string value, int notificationId, bool isUserSpecific)
    {
        Value = value;
        NotificationId = notificationId;
        IsUserSpecific = isUserSpecific;
    }

    public static NotificationTagContract PhotoApproved(int userId) 
        => CreateUserSpecificTag("PhotoApproved", 1, userId);
    public static NotificationTagContract NewWaitingPhoto() 
        => CreateGeneralTag("WaitingPhoto", 2);
    public static NotificationTagContract NewSimilarityRecord() 
        => CreateGeneralTag("NewSimilarityRecord", 3);
}
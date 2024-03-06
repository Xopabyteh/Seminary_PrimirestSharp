namespace Yearly.Contracts.Notifications;

public class NotificationTagContract
{
    public string Value { get; init; }
    public int NotificationId { get; init; } // >= 1!!!

    private NotificationTagContract(string value, int notificationId)
    {
        Value = value;
        NotificationId = notificationId;
    }


    public static string AssembleUserSpecificTag(NotificationTagContract tag, int userId)
        => $"{tag.Value}_{userId}";

    public static NotificationTagContract PhotoApproved => new("PhotoApproved", 1);
    public static NotificationTagContract NewWaitingPhoto => new("WaitingPhoto", 2);
    public static NotificationTagContract NewSimilarityRecord => new("SimilarityRecord", 3);
}
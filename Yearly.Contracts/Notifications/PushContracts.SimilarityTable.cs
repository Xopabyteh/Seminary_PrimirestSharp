namespace Yearly.Contracts.Notifications;
public static partial class PushContracts
{
    public class SimilarityTable
    {
        public const int k_SimilarityTableNotificationId = 103;
        public static string SimilarityTableTopic = CreateTopic("SimilarityTable", k_SimilarityTableNotificationId);
    }
}

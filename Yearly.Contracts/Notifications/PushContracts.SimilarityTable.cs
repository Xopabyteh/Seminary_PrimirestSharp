namespace Yearly.Contracts.Notifications;
public static partial class PushContracts
{
    public class SimilarityTable
    {
        public const int k_NewSimilarityRecordId = 201;
        public static string NewSimilarityRecordTopic = CreateTopic("SimilarityTable", k_NewSimilarityRecordId);
    }
}

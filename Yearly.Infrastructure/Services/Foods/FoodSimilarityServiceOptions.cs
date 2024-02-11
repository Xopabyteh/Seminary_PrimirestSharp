namespace Yearly.Infrastructure.Services.Foods;

public class FoodSimilarityServiceOptions
{
    public const string SectionName = "FoodSimilarity";
    public double NameStringSimilarityThreshold { get; set; } = 0.9d;
}
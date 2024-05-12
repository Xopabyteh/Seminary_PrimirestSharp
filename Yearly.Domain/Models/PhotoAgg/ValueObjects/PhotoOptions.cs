namespace Yearly.Domain.Models.PhotoAgg.ValueObjects;

public class PhotoOptions : ValueObject
{
    public const string SectionName = "Photo";

    /// <summary>
    /// Max size of side in pixels
    /// </summary>
    public int MaxSideLength { get; set; }

    /// <summary>
    /// Size of thumbnail in pixels
    /// </summary>
    public int ThumbnailSize { get; set; }

    public int PhotoJpegQuality { get; set; } = 75;

    public int ThumbnailJpegQuality { get; set; } = 75;

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return MaxSideLength;
        yield return ThumbnailSize;
    }
}
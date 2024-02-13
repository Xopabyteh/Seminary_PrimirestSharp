namespace Yearly.Contracts.Photos;

public record PhotoWithContextDTO(
    Guid Id,
    bool IsApproved,
    DateTime PublishDate,
    string ThumbnailResourceLink,
    string FoodName,
    string PublisherUsername);
namespace Yearly.Contracts.Photos;

public record WaitingPhotosResponse(List<PhotoDTO> Photos);

public record PhotoDTO(
    string PublisherUsername,
    DateTime PublishDate,
    string FoodName,
    string Link,
    Guid Id);


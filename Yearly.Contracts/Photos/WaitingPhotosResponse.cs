namespace Yearly.Contracts.Photos;

public record WaitingPhotosResponse(List<PhotoDTO> Photos);

public record PhotoDTO(
    Guid Id,
    string Link,
    DateTime PublishDate,
    string FoodName,
    string PublisherUsername);


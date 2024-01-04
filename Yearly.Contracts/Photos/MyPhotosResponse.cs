namespace Yearly.Contracts.Photos;

public readonly record struct MyPhotosResponse(List<string> FewLinks, int TotalPhotoCount);

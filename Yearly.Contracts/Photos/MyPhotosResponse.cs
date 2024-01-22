using Yearly.Contracts.Common;

namespace Yearly.Contracts.Photos;

public readonly record struct MyPhotosResponse(List<PhotoLinkDTO> Links, int TotalPhotoCount);

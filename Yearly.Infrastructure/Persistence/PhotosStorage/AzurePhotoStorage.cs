using Azure.Storage.Blobs;
using ErrorOr;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Infrastructure.Persistence.PhotosStorage;

public class AzurePhotoStorage : IPhotoStorage
{
    private const string k_ContainerName = "food-photos";

    private readonly BlobServiceClient _blobServiceClient;

    public AzurePhotoStorage(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<ErrorOr<string>> UploadPhotoAsync(Stream imageData, string name, string fileExtension)
    {
        var container = _blobServiceClient.GetBlobContainerClient(k_ContainerName);

        var resourceName = $"{name}.{fileExtension}";

        await container.UploadBlobAsync(resourceName, imageData);

        //Return link of uploaded photo
        return new Uri(Path.Combine(container.Uri.AbsoluteUri, resourceName)).AbsoluteUri;
    }

    public async Task DeletePhotoAsync(string resourceLink)
    {
        var container = _blobServiceClient.GetBlobContainerClient(k_ContainerName);
        var uri = new Uri(resourceLink);
        var blobUriBuilder = new BlobUriBuilder(uri);
        var blobName = blobUriBuilder.BlobName;

        await container.DeleteBlobAsync(blobName);
    }
}
using Azure.Storage.Blobs;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Infrastructure.Persistence.PhotosStorage;

public class AzurePhotoStorage : IPhotoStorage
{
    private const string k_ContainerName = "food-photos";

    private const string k_FileExtension = "jpg";
    
    /// <summary>
    /// Get file name with extension
    /// </summary>
    /// <param name="fromName"></param>
    /// <returns></returns>
    private static string GetFileName(string fromName)
        => $"{fromName}.{k_FileExtension}";

    private readonly BlobServiceClient _blobServiceClient;

    public AzurePhotoStorage(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<ErrorOr<string>> UploadPhotoAsync(IFormFile file, string name)
    {
        throw new NotImplementedException();

        //var container = _blobServiceClient.GetBlobContainerClient(k_ContainerName);

        //var photoStream = file.OpenReadStream();
        //var fileName = GetFileName(name);
        //await container.UploadBlobAsync(fileName, photoStream);

        ////Return link of uploaded photo
        //return $"{container.Uri.AbsoluteUri}/{fileName}";
    }

    public Task DeletePhotoAsync(string resourceLink)
    {
        throw new NotImplementedException();
        //var container = _blobServiceClient.GetBlobContainerClient(k_ContainerName);

        //var fileName = GetFileName(name);
        //return container.DeleteBlobAsync(fileName);
    }
}
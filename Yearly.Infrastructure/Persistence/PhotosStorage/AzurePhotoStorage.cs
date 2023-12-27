using System.Web;
using Azure.Storage.Blobs;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Infrastructure.Persistence.PhotosStorage;

public class AzurePhotoStorage : IPhotoStorage
{
    private const string k_ContainerName = "food-photos";

    //private const string k_FileExtension = "jpg";
    
    ///// <summary>
    ///// Get file name with extension
    ///// </summary>
    ///// <param name="fromName"></param>
    ///// <returns></returns>
    //private static string GetFileName(string fromName)
    //    => $"{fromName}.{k_FileExtension}";

    private readonly BlobServiceClient _blobServiceClient;

    public AzurePhotoStorage(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<ErrorOr<string>> UploadPhotoAsync(IFormFile file, string name)
    {
        var container = _blobServiceClient.GetBlobContainerClient(k_ContainerName);
        
        var fileExtension = Path.GetExtension(file.FileName);
        var resourceName = $"{name}{fileExtension}";
        var fileStream = file.OpenReadStream();

        await container.UploadBlobAsync(resourceName, fileStream);
        
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
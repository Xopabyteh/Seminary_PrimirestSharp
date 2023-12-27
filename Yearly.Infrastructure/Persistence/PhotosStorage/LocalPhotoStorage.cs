using ErrorOr;
using Microsoft.AspNetCore.Http;
using Yearly.Application.Common.Interfaces;

namespace Yearly.Infrastructure.Persistence.PhotosStorage;

public class LocalPhotoStorage : IPhotoStorage
{
    public async Task<ErrorOr<string>> UploadPhotoAsync(IFormFile file, string name)
    {
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");

        //Assure that directory exists
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, name);
        var fileResourceLink = new Uri(filePath, UriKind.Absolute); //Usable in http requests

        await using var stream = new FileStream(fileResourceLink.AbsolutePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileResourceLink.AbsolutePath;
    }

    public Task DeletePhotoAsync(string resourceLink)
    {
        var filePath = resourceLink;
        File.Delete(filePath);

        return Task.CompletedTask;
    }
}
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Yearly.Application.Common.Interfaces;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Infrastructure.Services.Photos;

public class LocalPhotoStorageService : IPhotoStorageService
{
    public async Task<ErrorOr<string>> UploadPhotoAsync(IFormFile file, PhotoId id)
    {
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");

        //Assure that directory exists
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, file.FileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }

    public Task DeletePhotoAsync(string path)
    {
        File.Delete(path);
        return Task.CompletedTask;
    }
}
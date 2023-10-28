using ErrorOr;
using Microsoft.AspNetCore.Http;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Application.Common.Interfaces;

public interface IPhotoStorageService
{
    /// <param name="file"></param>
    /// <param name="id">The id by which it is stored and then retrieved</param>
    /// <returns>A link to the photo resource</returns>
    public Task<ErrorOr<string>> UploadPhotoAsync(IFormFile file, PhotoId id);

    public Task DeletePhotoAsync(string path);
}
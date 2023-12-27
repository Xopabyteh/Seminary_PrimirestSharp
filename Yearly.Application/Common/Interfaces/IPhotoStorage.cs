using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Yearly.Application.Common.Interfaces;

public interface IPhotoStorage
{
    /// <param name="file"></param>
    /// <param name="name">The identifier by which the file is created and deleted - <b>Must be unique</b></param>
    /// <returns>A link to the photo resource</returns>
    public Task<ErrorOr<string>> UploadPhotoAsync(IFormFile file, string name);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resourceLink"></param>
    /// <returns>A Path to the resource</returns>
    public Task DeletePhotoAsync(string resourceLink);
}
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Photos.Commands;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Queries.DTORepositories;

namespace Yearly.Presentation.Controllers;

[Route("api/photo")]
public class PhotoController : ApiController
{
    private readonly PhotosDTORepository _photosDTORepository;
    public PhotoController(ISender mediator, PhotosDTORepository photosDTORepository)
        : base(mediator)
    {
        _photosDTORepository = photosDTORepository;
    }

    [HttpPost("publish")]
    public Task<IActionResult> UploadPhoto(
        [FromForm] IFormFile photo,
        [FromForm] Guid foodId,
        [FromHeader] string sessionCookie)
    {
        return PerformAuthenticatedActionAsync(sessionCookie, async user =>
        {
            if (user.Roles.Contains(UserRole.BlackListedFromTakingPhotos))
                Unauthorized();

            var result = await _mediator.Send(
                new PublishPhotoCommand(photo, new FoodId(foodId), user));

            return result.Match(
                createdPhoto => Created(createdPhoto.ResourceLink, null),
                Problem);
        });
    }

    [HttpPost("approve")]
    public Task<IActionResult> ApprovePhoto(
        [FromForm] Guid photoId,
        [FromHeader] string sessionCookie)
    {
        return PerformAuthorizedActionAsync(
            sessionCookie,
            async issuer =>
        {
            var result = await _mediator.Send(new ApprovePhotoCommand(new PhotoId(photoId), issuer));
            return result.Match(
                _ => Ok(),
                Problem);
        },
            UserRole.PhotoApprover);
    }

    [HttpPost("reject")]
    public Task<IActionResult> RejectPhoto(
        [FromForm] Guid photoId,
        [FromHeader] string sessionCookie)
    {
        return PerformAuthorizedActionAsync(
            sessionCookie,
            async issuer =>
        {
            var result = await _mediator.Send(new RejectPhotoCommand(new PhotoId(photoId), issuer));
            return result.Match(
                _ => Ok(),
                Problem);
        },
            UserRole.PhotoApprover);
    }

    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaitingPhotos()
    {
        var response = await _photosDTORepository.GetWaitingPhotosAsync();
        return Ok(response);
    }

    [HttpGet("my-photos")]
    public Task<IActionResult> GetMyPhotos([FromHeader] string sessionCookie)
    {
        return PerformAuthenticatedActionAsync(
            sessionCookie,
            async user =>
            {
                var userPhotos = await _photosDTORepository.GetUsersPhotos(user.Id.Value);
                return Ok(userPhotos);
            });
    }
}
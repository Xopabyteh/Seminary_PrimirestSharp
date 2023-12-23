using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Photos.Commands;
using Yearly.Contracts.Photos;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Queries.DTORepositories;

namespace Yearly.Presentation.Controllers;

[Route("photo")]
public class PhotoController : ApiController
{
    private readonly WaitingPhotosDTORepository _waitingPhotosDtoRepository;
    public PhotoController(ISender mediator, WaitingPhotosDTORepository waitingPhotosDtoRepository)
        : base(mediator)
    {
        _waitingPhotosDtoRepository = waitingPhotosDtoRepository;
    }

    [HttpPost("publish")]
    public Task<IActionResult> UploadPhoto(
        [FromForm] PublishPhotoRequest request,
        [FromHeader] string sessionCookie)
    {
        return PerformAuthenticatedActionAsync(sessionCookie, async user =>
        {
            if (user.Roles.Contains(UserRole.BlackListedFromTakingPhotos))
                Unauthorized();

            var result = await _mediator.Send(
                new PublishPhotoCommand(request.Photo, new FoodId(request.FoodId), user));

            return result.Match(
                createdPhoto => Created(createdPhoto.Link, null),
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
            async user =>
        {
            var result = await _mediator.Send(new ApprovePhotoCommand(new PhotoId(photoId), user));
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
            async user =>
        {
            var result = await _mediator.Send(new RejectPhotoCommand(new PhotoId(photoId), user));
            return result.Match(
                _ => Ok(),
                Problem);
        },
            UserRole.PhotoApprover);
    }

    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaitingPhotos()
    {
        var response = await _waitingPhotosDtoRepository.GetWaitingPhotosAsync();
        return Ok(response);
    }
}
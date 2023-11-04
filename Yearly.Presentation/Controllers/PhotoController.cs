using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Photos.Commands.Approve;
using Yearly.Application.Photos.Commands.Publish;
using Yearly.Application.Photos.Commands.Reject;
using Yearly.Application.Photos.Queries.Waiting;
using Yearly.Contracts.Photos;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("photo")]
public class PhotoController : ApiController
{
    public PhotoController(ISender mediator)
        : base(mediator)
    {
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

            var result = await _mediator.Send(new PublishPhotoCommand(photo, new FoodId(foodId), user));
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
        var result = await _mediator.Send(new WaitingPhotosQuery());

        var response = new WaitingPhotosResponse(result.Select(p => p.Id.Value).ToList());
        
        return Ok(response);
    }
}
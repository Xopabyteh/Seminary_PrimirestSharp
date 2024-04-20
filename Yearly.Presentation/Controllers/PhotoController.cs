using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Photos.Commands;
using Yearly.Application.Photos.Queries;
using Yearly.Contracts.Photos;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("api/photo")]
public class PhotoController : ApiController
{
    public PhotoController(ISender mediator)
        : base(mediator)
    {
    }

    [HttpPost("publish")]
    public Task<IActionResult> UploadPhoto(
        [FromForm] IFormFile photo,
        [FromForm] Guid foodId)
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            if (issuer.User.Roles.Contains(UserRole.BlackListedFromTakingPhotos))
                Unauthorized();

            var result = await _mediator.Send(
                new PublishPhotoCommand(photo, new FoodId(foodId), issuer.User));

            return result.Match(
                createdPhoto => Created(createdPhoto.ResourceLink, null),
                Problem);
        });
    }

    [HttpPost("approve")]
    public Task<IActionResult> ApprovePhoto([FromForm] Guid photoId)
    {
        return PerformAuthorizedActionAsync(
            async issuer =>
            {
                var result = await _mediator.Send(new ApprovePhotoCommand(new PhotoId(photoId), issuer.User));
                return result.Match(
                    _ => Ok(),
                    Problem);
            },
            UserRole.PhotoApprover);
    }

    [HttpPost("reject")]
    public Task<IActionResult> RejectPhoto([FromForm] Guid photoId)
    {
        return PerformAuthorizedActionAsync(
            async issuer =>
            {
                var result = await _mediator.Send(new RejectPhotoCommand(new PhotoId(photoId), issuer.User));
                return result.Match(
                    _ => Ok(),
                    Problem);
            },
            UserRole.PhotoApprover);
    }

    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaitingPhotos()
    {
        var waitingPhotos = await _mediator.Send(new GetWaitingPhotoDTOsQuery());
        return Ok(new WaitingPhotosResponse(waitingPhotos));
    }

    [HttpGet("my-photos")]
    public Task<IActionResult> GetMyPhotos()
    {
        return PerformAuthenticatedActionAsync(async issuer =>
        {
            var userPhotos = await _mediator.Send(new GetUsersPhotosDataFragmentQuery(issuer.User.Id));

            var response = new MyPhotosResponse(userPhotos.Data.ToList(), userPhotos.TotalCount);
            return Ok(response);
        });
    }
}
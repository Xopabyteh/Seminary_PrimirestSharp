using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yearly.Application.Photos.Commands;
using Yearly.Application.Photos.Commands.Approve;
using Yearly.Application.Photos.Commands.Publish;
using Yearly.Application.Photos.Commands.Reject;
using Yearly.Application.Photos.Queries.Waiting;
using Yearly.Contracts.Common;
using Yearly.Contracts.Photos;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;

namespace Yearly.Presentation.Controllers;

[Route("photo")]
public class PhotoController : ApiController
{
    private readonly ISender _mediator;
    public PhotoController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("publish")]
    public async Task<IActionResult> UploadPhoto(
        [FromForm] IFormFile photo, 
        [FromForm] Guid foodId,
        [FromHeader] string sessionCookie)
    {
        var result = await _mediator.Send(new PublishPhotoCommand(photo, new FoodId(foodId), sessionCookie));
        return result.Match(
            _ => Ok(), 
            Problem);
    }

    [HttpPost("approve")]
    public async Task<IActionResult> ApprovePhoto(
        [FromForm] Guid photoId,
        [FromHeader] string sessionCookie)
    {
        var result = await _mediator.Send(new ApprovePhotoCommand(new PhotoId(photoId), sessionCookie));
        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPost("reject")]
    public async Task<IActionResult> RejectPhoto(
        [FromForm] Guid photoId,
        [FromHeader] string sessionCookie)
    {
        var result = await _mediator.Send(new RejectPhotoCommand(new PhotoId(photoId), sessionCookie));
        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpGet("waiting")]
    public async Task<IActionResult> GetWaitingPhotos()
    {
        var result = await _mediator.Send(new WaitingPhotosQuery());

        var response = new WaitingPhotosResponse(result.Select(p => p.Id.Value).ToList());
        
        return Ok(response);
    }
}
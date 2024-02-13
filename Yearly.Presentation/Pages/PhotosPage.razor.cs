using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using MediatR;
using Microsoft.AspNetCore.Components;
using Yearly.Application.Photos.Commands;
using Yearly.Contracts.Photos;
using Yearly.Domain.Models.PhotoAgg.ValueObjects;
using Yearly.Domain.Models.UserAgg;
using Yearly.Queries.DTORepositories;

namespace Yearly.Presentation.Pages;

public partial class PhotosPage
{
    [Inject] private PhotosDTORepository _photosDTORepository { get; set; } = null!;
    [Inject] private ISender _mediator { get; set; } = null!;
    [Inject] private IHxMessengerService _messenger { get; set; } = null!;

    private HxGrid<PhotoWithContextDTO> gridComponent = null!; // @ref

    private async Task<GridDataProviderResult<PhotoWithContextDTO>> GetGridData(GridDataProviderRequest<PhotoWithContextDTO> request)
    {
        return new GridDataProviderResult<PhotoWithContextDTO>()
        {
            Data = await _photosDTORepository.GetPhotosWithContextAsync(request.Count!.Value, request.StartIndex, request.CancellationToken),
            TotalCount = await _photosDTORepository.GetTotalPhotosCountAsync(request.CancellationToken)
        };
    }

    private async Task HandleDeleteClick(PhotoWithContextDTO photoToDelete)
    {
        var command = new DeletePhotoCommand(new PhotoId(photoToDelete.Id));
        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            _messenger.AddError(result.FirstError.Code, result.FirstError.Description);
            return;
        }

        // -> Successfully deleted
        await gridComponent.RefreshDataAsync();
    }
    private async Task HandleApprovePhoto(PhotoWithContextDTO photoToDelete)
    {
        //var command = new ApprovePhotoCommand(new PhotoId(photoToDelete.Id), );
        //var result = await _mediator.Send(command);

        //if (result.IsError)
        //{
        //    _messenger.AddError(result.FirstError.Code, result.FirstError.Description);
        //    return;
        //}

        //// -> Successfully deleted
        //await gridComponent.RefreshDataAsync();
    }
    private async Task HandleRejectPhoto(PhotoWithContextDTO photoToDelete)
    {
        //var command = new DeletePhotoCommand(new PhotoId(photoToDelete.Id));
        //var result = await _mediator.Send(command);

        //if (result.IsError)
        //{
        //    _messenger.AddError(result.FirstError.Code, result.FirstError.Description);
        //    return;
        //}

        //// -> Successfully deleted
        //await gridComponent.RefreshDataAsync();
    }
}
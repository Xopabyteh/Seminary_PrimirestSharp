using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using MediatR;
using Microsoft.AspNetCore.Components;
using Yearly.Application.Foods.Commands.FoodSimilarity;
using Yearly.Application.Menus.Commands;
using Yearly.Contracts.Foods;
using Yearly.Domain.Models.FoodAgg.ValueObjects;
using Yearly.Queries.DTORepositories;

namespace Yearly.Presentation.Pages;

public partial class FoodsPage
{
    [Inject] private FoodDTORepository _foodDTORepository { get; set; } = null!;
    [Inject] private ISender _mediator { get; set; } = null!;
    [Inject] private IHxMessengerService _messenger { get; set; } = null!;

    private HxGrid<FoodWithContextDTO> grid = null!; // @ref
    private FoodDTORepository.FoodsWithContextFilter filterModel = new(nameFilter: string.Empty);

    private HxOffcanvas offcanvasComponent = null!; // @ref
    private FoodWithContextDTO editSelectedFood = null!; //@bind
    private string editInputAliasForFoodId = null!; //@bind

    private async Task<GridDataProviderResult<FoodWithContextDTO>> GetGridData(GridDataProviderRequest<FoodWithContextDTO> request)
    {
        return new GridDataProviderResult<FoodWithContextDTO>()
        {
            Data = await _foodDTORepository.GetFoodsWithContextAsync(
                filterModel,
                request.StartIndex,
                request.Count!.Value,
                request.CancellationToken),
            TotalCount = await _foodDTORepository.GetTotalFoodsCountAsync(
                filterModel,
                request.CancellationToken)
        };
    }

    private async Task HandleEditUserClick(FoodWithContextDTO item)
    {
        editSelectedFood = item;
        await offcanvasComponent.ShowAsync();
    }

    private async Task HandleSetAliasClick()
    {
        var didParse = Guid.TryParse(editInputAliasForFoodId, out var aliasOriginId);
        if (!didParse)
        {
            _messenger.AddError($"Invalid format, the input should the GUID of the Alias Origin (E.X. {Guid.NewGuid()}");
            return;
        }

        var command = new SetFoodAliasCommand(new FoodId(editSelectedFood.Id), new FoodId(aliasOriginId));
        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            _messenger.AddError(result.FirstError.Code, result.FirstError.Description);
            return;
        }
        
        // -> Successfully updated roles
        _messenger.AddMessage(new MessengerMessage()
        {
            AutohideDelay = 5000,
            Title = "Success",
            Icon = BootstrapIcon.Check
        });

        await offcanvasComponent.HideAsync();

        await grid.RefreshDataAsync();
    }

    private async Task HandleForcePersistClick()
    {
        var command = new PersistAvailableMenusCommand();
        var result = await _mediator.Send(command);
        if (result.IsError)
        {
            _messenger.AddError(result.FirstError.Code, result.FirstError.Description);
            return;
        }

        await grid.RefreshDataAsync();
        StateHasChanged();
    }
}
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using MediatR;
using Microsoft.AspNetCore.Components;
using Yearly.Application.Authentication.Commands;
using Yearly.Application.Users.Queries;
using Yearly.Contracts.Authentication;
using Yearly.Contracts.Users;
using Yearly.Domain.Models.UserAgg.ValueObjects;
using Yearly.Presentation.Pages.Services;

namespace Yearly.Presentation.Pages;

public partial class UsersPage
{
    [Inject] private ISender _mediator { get; set; } = null!;
    [Inject] private SessionDetailsService _sessionDetails { get; set; } = null!;
    [Inject] private IHxMessengerService _messenger { get; set; } = null!;

    private HxGrid<UserWithContextDTO> grid = null!; // @ref
    private UsersWithContextFilter filterModel = new(usernameFilter: string.Empty);

    private HxOffcanvas offcanvasComponent = null!; // @ref
    private UserWithContextDTO editSelectedUser;
    private List<UserRoleDTO> editSelectedRoles { get; set; } = new();

    private async Task<GridDataProviderResult<UserWithContextDTO>> GetGridData(GridDataProviderRequest<UserWithContextDTO> request)
    {
        var dataFragment = await _mediator.Send(
            new GetUsersWithContextDataFragmentQuery(
                filterModel,
                request.StartIndex,
                request.Count!.Value), 
            request.CancellationToken);

        return new GridDataProviderResult<UserWithContextDTO>()
        {
            Data = dataFragment.Data,
            TotalCount = dataFragment.TotalCount
        };
    }

    private async Task HandleEditUserClick(UserWithContextDTO item)
    {
        editSelectedUser = item;
        editSelectedRoles = editSelectedUser.Roles;
        await offcanvasComponent.ShowAsync();
    }

    private async Task HandleSaveClick()
    {
        var command = new UpdateUserRolesCommand(
            new UserId(editSelectedUser.Id),
            editSelectedRoles.Select(r => new UserRole(r.RoleCode)).ToList(),
            _sessionDetails.User!);
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
}
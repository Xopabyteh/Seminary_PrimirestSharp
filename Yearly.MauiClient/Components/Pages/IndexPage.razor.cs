using Microsoft.AspNetCore.Components;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient.Components.Pages;

public partial class IndexPage
{
    [Inject]
    public AuthService AuthService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        //The exception type that throws this message: "No connection could be made because the target machine actively refused it" is:
        try
        {
            var hasSession = await AuthService.TryLoadStoredSessionAsync();
            
            if (!hasSession)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }
        }
        catch (HttpRequestException e)
        {
            //Todo: 
            NavigationManager.NavigateTo("/login");
            return;
        }

        //We have a session, skip login
        NavigationManager.NavigateTo("/orders");
    }
}
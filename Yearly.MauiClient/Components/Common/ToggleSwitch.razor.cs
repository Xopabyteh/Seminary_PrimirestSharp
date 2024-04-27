using Microsoft.AspNetCore.Components;

namespace Yearly.MauiClient.Components.Common;

public partial class ToggleSwitch
{
    [Parameter] public bool Checked { get; set; } = false;
    [Parameter] public bool StartingState { get; set; } = false;

    /// <summary>
    /// In: Desired state
    /// Out: What should be the result state
    /// </summary>
    [Parameter] public Func<bool, Task<bool>> OnTryCheckedChange { get; set; }

    protected override void OnInitialized()
    {
        Checked = StartingState;
    }

    private async Task ChangeChecked()
    {
        Checked = await OnTryCheckedChange(!Checked);
        StateHasChanged();
    }
}
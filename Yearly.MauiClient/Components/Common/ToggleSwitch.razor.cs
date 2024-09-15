using Microsoft.AspNetCore.Components;

namespace Yearly.MauiClient.Components.Common;

public partial class ToggleSwitch
{
    [Parameter] public bool IsChecked { get; set; } = false;
    /// <summary>
    /// If true, the button will predict it's next state and validate it after
    /// callback has been executed, which means it will switch instantly, be unusable until calculated
    /// and then switch back.
    /// When false, the button will pause until callback completes and then use given state.
    /// </summary>
    private bool waitingForCallbackResult = false;
    private bool IsDisabled => waitingForCallbackResult;

    /// <summary>
    /// In: Desired state
    /// Out: What should be the result state
    /// </summary>
    [Parameter] public Func<bool, Task<bool>>? OnCheckedChanged { get; set; }

    protected override bool ShouldRender()
    {
        // Don't re-render when waiting for callback
        // This re-render might be caused when parameters are passed
        // back in to the toggle through the parent.
        return !IsDisabled;
    }

    private async Task ChangeChecked()
    {
        if (IsDisabled)
            return;

        IsChecked = !IsChecked;
        StateHasChanged();

        if (OnCheckedChanged is null)
            return;

        waitingForCallbackResult = true;
        IsChecked = await OnCheckedChanged(IsChecked);
        waitingForCallbackResult = false;
    }
}
namespace Yearly.MauiClient.Components.Layout;

public partial class NavigationBar
{
    private bool isShowing = true;

    public void Show()
    {
        isShowing = true;
    }

    public void Hide()
    {
        isShowing = false;
    }
}
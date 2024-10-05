namespace Yearly.MauiClient.Components.Layout;

public partial class ThemeManager
{
    private const string k_ThemePrefKey = "isdarktheme";
    public static ThemeManager? Instance { get; private set; }

    protected override void OnInitialized()
    {
        Instance = this;

        isDarkMode = Preferences.Get(k_ThemePrefKey, false);
    }

    private bool isDarkMode;

    public bool IsDarkMode
    {
        get => isDarkMode;
        set
        {
            isDarkMode = value;

            Preferences.Set(k_ThemePrefKey, isDarkMode);

            StateHasChanged();
        }
    }
}
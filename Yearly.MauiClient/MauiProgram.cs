using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddLogging(c =>
        {
            c.AddDebug();
        });
#endif

        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<SharpAPIFacade>();

        return builder.Build();
    }
}
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.LocalNotification;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
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


        builder.Services.AddTransient<ToastService>();

        builder.Services.AddSingleton<MenuAndOrderCacheService>();
        
        //Sharp API
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<SharpAPIClient>();

        builder.Services.AddTransient<MenusFacade>();
        builder.Services.AddTransient<AuthenticationFacade>();
        builder.Services.AddTransient<OrdersFacade>();
        builder.Services.AddTransient<PhotoFacade>();

        return builder.Build();
    }
}
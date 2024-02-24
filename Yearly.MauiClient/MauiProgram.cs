using CommunityToolkit.Maui;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;

namespace Yearly.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

#if ANDROID || IOS
        builder.UseLocalNotification();
#endif

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddLogging(c =>
        {
            c.AddDebug();
        });
#endif
        builder.Services.AddTransient<HistoryService>();

        builder.Services.AddTransient<DateTimeProvider>();

        builder.Services.AddTransient<ToastService>();

        builder.Services.AddTransient<OrderCheckerService>();

        builder.Services.AddSingleton<MenuAndOrderCacheService>();
        builder.Services.AddSingleton<MyPhotosCacheService>();

        //Sharp API
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<SharpAPIClient>();
        builder.Services.AddSingleton<WebRequestProblemService>();

        builder.Services.AddTransient<MenusFacade>();
        builder.Services.AddTransient<AuthenticationFacade>();
        builder.Services.AddTransient<OrdersFacade>();
        builder.Services.AddTransient<PhotoFacade>();
        builder.Services.AddTransient<FoodFacade>();

        return builder.Build();
    }
}
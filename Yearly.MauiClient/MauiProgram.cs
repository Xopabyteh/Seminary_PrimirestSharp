using CommunityToolkit.Maui;
#if DEBUG
using Microsoft.Extensions.Logging;
#endif
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;
using Shiny;
#if ANDROID || IOS
using Plugin.LocalNotification;
using Shiny.Push;
using Shiny.Jobs;
using Yearly.MauiClient.Services.Notifications;
#endif

namespace Yearly.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShiny()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

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

        // Jobs
#if ANDROID || IOS
        builder.Services.AddJobs();
#endif
        // Notifications
#if ANDROID || IOS
        builder.UseLocalNotification();
#if ANDROID
            var firebaseCfg = new FirebaseConfig(
                false,
                "1:32637295511:android:77db4b6fdcd37a706e42d2",
                "32637295511",
                "primirest-sharp-fb",
                "AIzaSyAG4HEPPOnup-0SvazStty9nKFkrOwqgR0"
            );
#endif

            builder.Services.AddPushAzureNotificationHubs<MyPushDelegate>(
                "Endpoint=sb://PrimirestSharpNS.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=S1EiLQf9Ai0X5t7SCkWlfq1grCCOlO57mGtEgsOp3/0=",
                "PrimirestSharpNH"
#if ANDROID
                , firebaseCfg
#endif
            );
#endif


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddLogging(c =>
        {
            c.AddDebug();
        });

#endif

        return builder.Build();
    }
}
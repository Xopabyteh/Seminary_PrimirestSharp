using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;
using Shiny;
#if ANDROID || IOS
using Plugin.LocalNotification;
using Shiny.Push;
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
#if ANDROID || IOS
        builder.UseLocalNotification();
#endif
        ////Config (add appsettings.json)
        //builder.Configuration.AddJsonFile("appsettings.json", optional: false);

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddLogging(c =>
        {
            c.AddDebug();
        });

#endif
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
        //Notifications
#if ANDROID || IOS
#if ANDROID
            var firebaseCfg = new FirebaseConfig(
                false,
                "1:32637295511:android:77db4b6fdcd37",
                "32637295511",
                "primirest-sharp-fb",
                "AIzaSyAG4HEPPOnup-0SvazStty9nKFkrOwqgR0"
            );
#endif

            builder.Services.AddPushAzureNotificationHubs<MyPushDelegate>(
                "Endpoint=sb://PrimirestSharpNotificationHubNS.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=jbGQazmRlrCHqKOyIO+/UoA5YeY0PcfCiKx0xqBA6Ys=",
                "PrimirestSharpNH"
#if ANDROID
                , firebaseCfg
#endif
            );
#endif
        //#if ANDROID || IOS
        //#if ANDROID
        //            var cfg = builder.Configuration.GetSection("Firebase");
        //            var firebaseCfg = new FirebaseConfig(
        //                false,
        //                cfg["AppId"],
        //                cfg["SenderId"],
        //                cfg["ProjectId"],
        //                cfg["ApiKey"]
        //            );
        //#endif

        //            var azureCfg = builder.Configuration.GetSection("AzureNotificationHubs");
        //            builder.Services.AddPushAzureNotificationHubs<MyPushDelegate>(
        //                azureCfg["ListenerConnectionString"]!,
        //                azureCfg["HubName"]!
        //#if ANDROID
        //                , firebaseCfg
        //#endif
        //            );
        //#endif

        return builder.Build();
    }
}
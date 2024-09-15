using CommunityToolkit.Maui;
using Plugin.LocalNotification.AndroidOption;
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
using Yearly.MauiClient.Services.Notifications;
#if ANDROID
using Android.App;
#endif

#endif

namespace Yearly.MauiClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // ** Deploy Environment **
        var environment = DeployEnvironment.Prod;
        // ** Deploy Environment **

        var builder = MauiApp.CreateBuilder();
        return builder
            .UseMauiApp<App>()
            .UseShiny()
            .UseMauiCommunityToolkit()
            .RegisterServices(environment)
#if ANDROID || IOS
            .RegisterNotifications()
#endif
            .Build();
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder, DeployEnvironment environment)
    {
        builder.Services.AddSingleton(new DeployEnvironmentAccessor(environment));
        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddTransient<DateTimeProvider>();

        builder.Services.AddTransient<ToastService>();

        builder.Services.AddTransient<OrderCheckerService>();

        builder.Services.AddSingleton<MenuAndOrderCacheService>();
        builder.Services.AddSingleton<MyPhotosCacheService>();

        // Sharp API
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiUrlService>();
        builder.Services.AddSingleton<SharpAPIClient>();
        builder.Services.AddSingleton<WebRequestProblemService>();

        builder.Services.AddTransient<MenusFacade>();
        builder.Services.AddTransient<AuthenticationFacade>();
        builder.Services.AddTransient<OrdersFacade>();
        builder.Services.AddTransient<PhotoFacade>();
        builder.Services.AddTransient<FoodFacade>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddLogging(c =>
        {
            c.AddDebug();
        });
#endif

        // Jobs
#if ANDROID || IOS
        builder.Services.AddJobs();
#endif

        return builder;
    }

#if ANDROID || IOS
    public static MauiAppBuilder RegisterNotifications(this MauiAppBuilder builder)
    {
        builder.UseLocalNotification(c =>
        {
            c.AddAndroid(ac =>
            {
                ac.AddChannel(new NotificationChannelRequest() {Id = "orders_notifications", Name = "Objednávky"});
            });
        });
        
#if ANDROID
        NotificationChannel serverNotificationsChannel = new(
            "server_notifications",
            "Server notifications",
            NotificationImportance.Default)
        {
            LockscreenVisibility = NotificationVisibility.Private
        };
        
        var firebaseCfg = new FirebaseConfig(
            false,
            "1:32637295511:android:77db4b6fdcd37a706e42d2",
            "32637295511",
            "primirest-sharp-fb",
            "AIzaSyAG4HEPPOnup-0SvazStty9nKFkrOwqgR0",
            serverNotificationsChannel
        );
#endif

        builder.Services.AddPushAzureNotificationHubs<MyPushDelegate>(
            "Endpoint=sb://PrimirestSharpNotificationHubNamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=KK4SF5WMDV5dfbEHbomTiuiTHCC4atESqC4hDJCiKfI=",
            "PrimirestSharpNotificationHub"
#if ANDROID
            , firebaseCfg
#endif
        );

        return builder;
    }
#endif
}
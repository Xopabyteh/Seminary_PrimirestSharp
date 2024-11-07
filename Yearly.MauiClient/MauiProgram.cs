using CommunityToolkit.Maui;
using Plugin.LocalNotification.AndroidOption;
#if DEBUG
using Microsoft.Extensions.Logging;
#endif
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApi;
using Yearly.MauiClient.Services.SharpApi.Facades;
using Yearly.MauiClient.Services.Toast;
using Plugin.LocalNotification;
using Yearly.Contracts.Notifications;
using Plugin.Firebase.CloudMessaging;
using Yearly.MauiClient.Services.Notifications;
using Yearly.MauiClient.Services.Notifications.Photos;
using Yearly.MauiClient.Services.Notifications.SimilarityTable;

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
            .UseMauiCommunityToolkit()
            .RegisterServices(environment)
            .RegisterNotifications()
            .Build();
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder, DeployEnvironment environment)
    {
        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddSingleton(new DeployEnvironmentAccessor(environment));
        builder.Services.AddSingleton<DateTimeProvider>();
        builder.Services.AddSingleton<ToastService>();

        // Push notifications
        builder.Services.AddSingleton<PushRegistrationService>();
        builder.Services.AddSingleton<PushNotificationHandlerService>();
        builder.Services.AddSingleton<IPushNotificationHandlerService, NewWaitingPhotoNotificationHandler>();
        builder.Services.AddSingleton<IPushNotificationHandlerService, PhotoApprovedNotificationHandler>();
        builder.Services.AddSingleton<IPushNotificationHandlerService, NewSimilarityRecordNotificationHandler>();

        builder.Services.AddTransient<OrderCheckerService>();

        builder.Services.AddSingleton<MenuAndOrderCacheService>();
        builder.Services.AddSingleton<MyPhotosCacheService>();

        // Sharp API
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiUrlService>();
        builder.Services.AddSingleton<SharpAPIClient>();
        builder.Services.AddSingleton<WebRequestProblemService>();

        builder.Services.AddSingleton<MenusFacade>();
        builder.Services.AddSingleton<AuthenticationFacade>();
        builder.Services.AddSingleton<OrdersFacade>();
        builder.Services.AddSingleton<PhotoFacade>();
        builder.Services.AddSingleton<FoodFacade>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddLogging(c =>
        {
            c.AddDebug();
        });
#endif

        return builder;
    }

    public static MauiAppBuilder RegisterNotifications(this MauiAppBuilder builder)
    {
        builder.UseLocalNotification(c =>
        {
            #if ANDROID
            c.AddAndroid(ac =>
            {
                ac.AddChannel(new NotificationChannelRequest()
                {
                    Id = PushContracts.General.k_GeneralNotificationChannelId,
                    Name = "General",
                });
                FirebaseCloudMessagingImplementation.ChannelId = PushContracts.General.k_GeneralNotificationChannelId;
            });
            #endif
        });

        #if ANDROID
        // Add FCM push notification handler for Android
        FirebaseCloudMessagingImplementation.ShowLocalNotificationAction = ShowFCMNotification;
        CrossFirebaseCloudMessaging.Current.TokenChanged += FCMTokenChanged;
        #endif

        return builder;
    }


#if ANDROID
    private static void FCMTokenChanged(object? sender, Plugin.Firebase.CloudMessaging.EventArgs.FCMTokenChangedEventArgs e)
    {
        Console.WriteLine($"P# FCM Token changed, Reg Token: {e.Token}");
        Preferences.Set(PushNotificationHandlerService.k_FCMTokenPrefKey, e.Token);
    }
    private static async void ShowFCMNotification(FCMNotification notification)
    {
        Console.WriteLine("P# FCM Message received");

        var pushHandlerService = IPlatformApplication.Current!.Services.GetService<PushNotificationHandlerService>()!;
        await pushHandlerService.HandleNotificationAsync(notification.Data ?? new Dictionary<string, string>(0));
    }
    #endif
}
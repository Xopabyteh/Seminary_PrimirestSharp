using Android.App;
using Android.Content;
using Android.OS;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Yearly.Contracts.Authentication;
using Yearly.Contracts.Menu;
using Yearly.Contracts.Order;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient;

[Service(Enabled = true, Exported = true)]
public class OrderCheckerBackgroundService : Service
{
    public const string IntentActionStart = "START";
    public const string IntentActionStop = "STOP";
    public static bool IsRunning { get; private set; }

    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService), "OnStartCommand called");

        //https://developer.android.com/reference/android/app/Service#START_STICKY
        if (intent is null)
        { 
            Start();   
        }
        else
        {
            switch (intent.Action)
            {
                case IntentActionStart:
                    Start();
                    break;
                case IntentActionStop:
                    Stop();
                    break;
            }
        }

        return StartCommandResult.Sticky;
    }

    private void Start()
    {
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService), "StartService called");
        if (IsRunning)
            return;

        IsRunning = true;
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService), "Starting timers");

        _ = new System.Threading.Timer(TimerTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        _ = new System.Threading.Timer(ShowWeAliveBaby, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
    }

    private void Stop()
    {
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService), "StopService");
        IsRunning = false;
        StopSelf();
    }

    private void ShowWeAliveBaby(object? state)
    {
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService), "We alive baby");
    }

    private async void TimerTick(object? state)
    {
        IsRunning = true;
        
        //Log
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService), "TimerTick");

        var authService = IPlatformApplication.Current!.Services.GetService<AuthService>()!;
        var authFacade = IPlatformApplication.Current!.Services.GetService<AuthenticationFacade>()!;

        if (authService.IsLoggedIn)
            return; //Don't try to trigger this, when viewing the app
                    // 1. The user himself can see whether he has ordered stuff already
                    // 2. It will mess up authentication

        //Login for checking session
        await authService.EnsureAutoLoginStateLoadedAsync();
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService),
            $"Auto login setup: {authService.IsAutoLoginSetup}");

        if (!authService.IsAutoLoginSetup)
            return;

        var loginResult = await authFacade.LoginAsync(authService.AutoLoginStoredCredentials!);
        if (loginResult.IsT1)
        {
            //Problem -> Auto login failed

            Android.Util.Log.Debug(nameof(OrderCheckerBackgroundService),
                "Login through Auto Login failed, stopping");
            StopSelf();
            return;
        }

        var loginResponse = loginResult.AsT0;
        await authService.SetSessionAsync(loginResponse);

        //Check
        await CheckIfHasOrdered();

        //Remove session
        await authService.RemoveSessionAsync();
    }

    private async Task CheckIfHasOrdered()
    {
        var orderCheckerService = IPlatformApplication.Current!.Services.GetService<OrderCheckerService>()!;
        var daysWithoutOrder = await orderCheckerService.GetDaysWithoutOrder();
        foreach (var day in daysWithoutOrder)
        {
            var notification = new NotificationRequest
            {
                NotificationId = day.Date.GetHashCode(),
                Title = "Nemáte objednáno",
                Description = $"Nemáte objednáno na {day.Date:dddd dd.MMMM}",
                Android = new AndroidOptions()
                {
                    IconSmallName = new AndroidIcon("appicon")
                }
            };

            await LocalNotificationCenter.Current.Show(notification);
        }
    }
}
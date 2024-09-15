using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Yearly.MauiClient.Components.Common;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient;

[Activity(
    LaunchMode = LaunchMode.SingleTop,
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
    new[] {
        Shiny.ShinyNotificationIntents.NotificationClickAction
    },
    Categories = new[] {
        "android.intent.category.DEFAULT"
    }
)]
public class MainActivity : MauiAppCompatActivity
{
    public static MainActivity Instance { get; private set; } = null!;

    public const int OrderCheckerBatteryRequestCode = 137; //Random number
    public TaskCompletionSource? OrderCheckerBatteryOptimizationResult;

    public MainActivity()
    {
        Instance = this;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        //Xamarin essentials
        Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle
    }
    
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        //Xamarin essentials
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        switch (requestCode)
        {
            case OrderCheckerBatteryRequestCode:
                if (OrderCheckerBatteryOptimizationResult is not null)
                {
                    OrderCheckerBatteryOptimizationResult.SetResult();
                    OrderCheckerBatteryOptimizationResult = null;
                }
                break;
        }
    }

    /// <summary>
    /// Reroutes the Back key to navigate back in history
    /// </summary>
    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        if (e is {KeyCode: Keycode.Back, Action: KeyEventActions.Down})
        {
            //Back key pressed, go back in history
            HistoryManager.Instance.TryGoBack();

            return true;
        }
        return base.DispatchKeyEvent(e);
    }

    protected override void OnDestroy()
    {
        Log.Debug(nameof(MainActivity), "PSHARP OnDestroy");

        var authService = IPlatformApplication.Current!.Services.GetService<AuthService>();

        if (authService is null)
        {
            base.OnDestroy();
            throw new ArgumentNullException(nameof(authService), "No auth service found");
        }

        if (!authService.IsLoggedIn)
        {
            base.OnDestroy();
            return;
        }

        authService.RemoveSessionAsync().GetAwaiter().GetResult();

        Log.Debug(nameof(MainActivity), "PSHARP Cleared Session");

        base.OnDestroy();
    }
}
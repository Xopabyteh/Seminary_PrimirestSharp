using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using Firebase;
using WindowsAzure.Messaging.NotificationHubs;

namespace Yearly.MauiClient;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static MainActivity Instance { get; private set; }

    public MainActivity()
    {
        Instance = this;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            ActivityCompat.RequestPermissions(
                this,
                new[] { Manifest.Permission.PostNotifications },
                0);
        }

        //Xamarin essentials
        Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle

        //Azure notification hub
        NotificationHub.SetListener(new AzureNotificationsListener());
        NotificationHub.Start(this.Application, AzureNotificationsListener.HubName, AzureNotificationsListener.ListenConnectionString);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
    {
        //Xamarin essentials
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    public void StartOrderCheckerBackgroundService()
    {
        if (OrderCheckerBackgroundService.IsRunning)
            return;

        //If version is < 26.0, return
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        var intent = new Intent(this, typeof(OrderCheckerBackgroundService));
        intent.SetAction(OrderCheckerBackgroundService.IntentActionStart);
        StartService(intent);
    }

    public void StopOrderCheckerBackgroundService()
    {
        var intent = new Intent(this, typeof(OrderCheckerBackgroundService));
        intent.SetAction(OrderCheckerBackgroundService.IntentActionStop);
        StartService(intent);
    }
}
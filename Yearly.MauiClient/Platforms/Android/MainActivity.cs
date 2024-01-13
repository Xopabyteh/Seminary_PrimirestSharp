using Android;
using Android.App;
using Android.Content.PM;
using Android.Icu.Util;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using WindowsAzure.Messaging.NotificationHubs;
using Calendar = Android.Icu.Util.Calendar;
using Locale = Java.Util.Locale;
using TimeUnit = Java.Util.Concurrent.TimeUnit;
using TimeZone = Android.Icu.Util.TimeZone;

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

    public void StartOrderCheckerBackgroundWorker()
    {
        var workManager = WorkManager.GetInstance(this)!;
        var czechLocale = new Locale("cs", "CZ");
        var czechTimezone = TimeZone.GetTimeZone("Europe/Prague");
        var currentDate = Calendar.GetInstance(czechTimezone, czechLocale)!;

        //OrderChecker Background Worker (Every day at 10:00 AM czech time)
        var ocDate = Calendar.GetInstance(czechTimezone, czechLocale)!;
        ocDate.Set(CalendarField.HourOfDay, 11); //Todo: change
        ocDate.Set(CalendarField.Minute, 30);
        ocDate.Set(CalendarField.Second, 0);
        if (ocDate.Before(currentDate)) //Move next execution for tomorrow, if it's too late for today
        {
            ocDate.Add(CalendarField.HourOfDay, 24);
        }
        var initialOffsetMillis = ocDate.TimeInMillis - currentDate.TimeInMillis;

        var ocConstraints = new Constraints.Builder()
            .SetRequiredNetworkType(NetworkType.Connected!)
            .Build();

        var ocBuilder = new PeriodicWorkRequest.Builder
                (typeof(OrderCheckerBackgroundWorker), TimeSpan.FromMinutes(15))
            .SetConstraints(ocConstraints);
            //.SetInitialDelay(initialOffsetMillis, TimeUnit.Milliseconds!)!;

        var ocRequest = (PeriodicWorkRequest)ocBuilder.Build();

        workManager.EnqueueUniquePeriodicWork(
            OrderCheckerBackgroundWorker.UniqueWorkerName,
            ExistingPeriodicWorkPolicy.Keep!,
            ocRequest);
    }

    public void StopOrderCheckerBackgroundWorker()
    {
        var workManager = WorkManager.GetInstance(this)!;
        workManager.CancelUniqueWork(OrderCheckerBackgroundWorker.UniqueWorkerName);
    }
}
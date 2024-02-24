using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Icu.Util;
using Android.OS;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using Java.Lang;
using Java.Util.Concurrent;
using WindowsAzure.Messaging.NotificationHubs;
using Yearly.MauiClient.Services;
using Calendar = Android.Icu.Util.Calendar;
using Debug = System.Diagnostics.Debug;
using Locale = Java.Util.Locale;
using NetworkType = AndroidX.Work.NetworkType;
using TimeUnit = Java.Util.Concurrent.TimeUnit;
using TimeZone = Android.Icu.Util.TimeZone;

namespace Yearly.MauiClient;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static MainActivity Instance { get; private set; } = null!;

    private const int k_NotificationsPermissionRequestCode = 136; //Random number
    private TaskCompletionSource<bool>? notificationsPermissionResult;
    
    private const int k_OrderCheckerBatteryRequestCode = 137; //Random number
    private TaskCompletionSource? orderCheckerBatteryOptimizationResult;

    public MainActivity()
    {
        Instance = this;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        //Xamarin essentials
        Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle

        //Azure notification hub
        NotificationHub.SetListener(new AzureNotificationsListener());
        NotificationHub.Start(this.Application, AzureNotificationsListener.HubName, AzureNotificationsListener.ListenConnectionString);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        //Xamarin essentials
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        switch (requestCode)
        {
            case k_NotificationsPermissionRequestCode:
                if (notificationsPermissionResult is not null)
                {
                    var result = false;
                    if (grantResults.Length > 0)
                    {
                        result = grantResults[0] == Permission.Granted;
                    }

                    notificationsPermissionResult.SetResult(result);
                    notificationsPermissionResult = null;
                }
                break;
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        switch (requestCode)
        {
            case k_OrderCheckerBatteryRequestCode:
                if (orderCheckerBatteryOptimizationResult is not null)
                {
                    orderCheckerBatteryOptimizationResult.SetResult();
                    orderCheckerBatteryOptimizationResult = null;
                }
                break;
        }
    }

    /// <summary>
    /// Tries to request notifications permission
    /// </summary>
    /// <returns>True if permission was granted, False if not</returns>
    private async Task<bool> TryRequestNotificationsPermission()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return true; // Notifications are always enabled on older versions

        if (ContextCompat.CheckSelfPermission(this, "android.permission.POST_NOTIFICATIONS") == Permission.Granted)
            return true; // Already granted

        // -> Request
        
        notificationsPermissionResult = new TaskCompletionSource<bool>();
        ActivityCompat.RequestPermissions(
            this,
            new[] { "android.permission.POST_NOTIFICATIONS" },
            k_NotificationsPermissionRequestCode);

        var result = await notificationsPermissionResult.Task;
        return result;
    }


    /// <summary>
    /// </summary>
    /// <param name="initialDelayMillis">If null, use delay that syncs the task to run daily from time specified by the domain rules</param>
    public async Task<bool> TryStartOrderCheckerAsync(long? initialDelayMillis = null)
    {
        if (!(await TryRequestNotificationsPermission()))
            return false;

        //Request "android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS"
        //so that the background work can run in the background
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            var packageName = PackageName;
            var pm = GetSystemService(PowerService) as PowerManager;

            orderCheckerBatteryOptimizationResult = new TaskCompletionSource();

            if (!(pm!.IsIgnoringBatteryOptimizations(packageName)))
            {
                //Show request prompt and detect whether the user pressed accept or deny
                var intent = new Intent();
                intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                StartActivityForResult(intent, k_OrderCheckerBatteryRequestCode);

                //Wait until accept or deny
                await orderCheckerBatteryOptimizationResult.Task;

                //Check if the user accepted
                if (!(pm.IsIgnoringBatteryOptimizations(packageName)))
                {
                    //Don't start and return false...
                    return false;
                }
            }
        }

        //This version doesn't require such permissions or we have them already
        //Start and return true
        ScheduleOrderChecker(initialDelayMillis);
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="initialDelayMillis">If null, use delay that syncs the task to run daily from time specified by the domain rules</param>
    private void ScheduleOrderChecker(long? initialDelayMillis = null)
    {
        Android.Util.Log.Debug(nameof(MainActivity), "Starting Order Checker BW");
        var workManager = WorkManager.GetInstance(this);
        var czechLocale = new Locale("cs", "CZ");
        var czechTimezone = TimeZone.GetTimeZone("Europe/Prague");
        var currentDate = Calendar.GetInstance(czechTimezone, czechLocale)!;

        //OrderChecker Background Worker (Every day at 10:00 AM czech time)
        var ocDate = Calendar.GetInstance(czechTimezone, czechLocale)!;
        ocDate.Set(CalendarField.HourOfDay, 10);
        ocDate.Set(CalendarField.Minute, 0);
        ocDate.Set(CalendarField.Second, 0);
        if (ocDate.Before(currentDate)) //Move next execution for tomorrow, if it's too late for today
        {
            ocDate.Add(CalendarField.HourOfDay, 24);
        }
        var initialOffsetMillis = initialDelayMillis ?? ocDate.TimeInMillis - currentDate.TimeInMillis;
        Android.Util.Log.Debug(nameof(MainActivity), $"Initial offset {initialOffsetMillis} millis");

        var ocConstraints = new Constraints.Builder()
            .SetRequiredNetworkType(NetworkType.Connected!)
            .SetRequiresCharging(false)
            .SetRequiresBatteryNotLow(false)
            .SetRequiresDeviceIdle(false)
            .SetRequiresStorageNotLow(false)
            .Build();

        var ocBuilder = new PeriodicWorkRequest.Builder
                (typeof(OrderCheckerBackgroundWorker), TimeSpan.FromDays(1))
            .SetConstraints(ocConstraints)
            .AddTag(OrderCheckerBackgroundWorker.WorkNameTag)
            .SetInitialDelay(initialOffsetMillis, TimeUnit.Milliseconds!)!;

        var ocRequest = (PeriodicWorkRequest)ocBuilder.Build();

        workManager.EnqueueUniquePeriodicWork(
            OrderCheckerBackgroundWorker.UniqueWorkerName,
            ExistingPeriodicWorkPolicy.Keep!,
            ocRequest);
    }

    public void StopOrderChecker()
    {
        Android.Util.Log.Debug(nameof(MainActivity), "Stopping Order Checker BW");
        var workManager = WorkManager.GetInstance(this);
        workManager.CancelAllWorkByTag(OrderCheckerBackgroundWorker.WorkNameTag); //Cancels rescheduled BWs as well
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if there is any order checker work scheduled/active</returns>
    public bool GetIsOrderCheckerBackgroundWorkerScheduled()
    {
        WorkManager instance = WorkManager.GetInstance(this);
        //ListenableFuture<List<WorkInfo>> statuses = instance.getWorkInfosByTag(tag);
        IListenableFuture statuses = instance.GetWorkInfosByTag(OrderCheckerBackgroundWorker.WorkNameTag);
        try
        {
            var running = false;
            dynamic workInfoList = statuses.Get()!;
            foreach (WorkInfo workInfo in workInfoList)
            {
                WorkInfo.State state = workInfo.GetState();
                running = state == WorkInfo.State.Running! | state == WorkInfo.State.Enqueued;
            }
            return running;
        }
        catch (ExecutionException e)
        {
            Android.Util.Log.Debug(nameof(MainActivity), "Error getting work info - stack trace: {0}", e.StackTrace);
            return false;
        }
        catch (InterruptedException e)
        {
            Android.Util.Log.Debug(nameof(MainActivity), "Error getting work info - stack trace: {0}", e.StackTrace);
            return false;
        }
    }

    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        if (e is {KeyCode: Keycode.Back, Action: KeyEventActions.Down})
        {
            //Back key pressed, go back in history
            var historyService = IPlatformApplication.Current!.Services.GetService<HistoryService>()!;
            historyService.TryGoBackAsync().GetAwaiter().GetResult();

            return true;
        }
        return base.DispatchKeyEvent(e);
    }
}
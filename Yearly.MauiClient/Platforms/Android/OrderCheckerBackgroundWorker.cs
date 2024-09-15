using _Microsoft.Android.Resource.Designer;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using Java.Lang;
using Java.Util.Concurrent;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;
using Android.Icu.Util;
using Calendar = Android.Icu.Util.Calendar;
using Locale = Java.Util.Locale;
using NetworkType = AndroidX.Work.NetworkType;
using TimeUnit = Java.Util.Concurrent.TimeUnit;
using TimeZone = Android.Icu.Util.TimeZone;


namespace Yearly.MauiClient;

public class OrderCheckerBackgroundWorker : Worker
{
    public const string UniqueWorkerName = $"psharp-{nameof(OrderCheckerBackgroundWorker)}";
    private const string k_UniqueRescheduledWorkerName = $"psharp-{nameof(OrderCheckerBackgroundWorker)}-rescheduled";
    public const string WorkNameTag = $"psharp-{nameof(OrderCheckerBackgroundWorker)}-tag";


    public OrderCheckerBackgroundWorker(Context context, WorkerParameters workerParams) 
        : base(context, workerParams)
    {
    }

    public override AndroidX.Work.ListenableWorker.Result DoWork()
    {
        //Log
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "DoWork");

        var authService = IPlatformApplication.Current!.Services.GetService<AuthService>()!;

        if (authService.IsLoggedIn)
        {
            //Don't try to trigger this, when viewing the app
            // 1. The user himself can see whether he has ordered stuff already
            // 2. It will mess up authentication

            //Reschedule self in 15 minutes
            var workManager = WorkManager.GetInstance(ApplicationContext);
            var workRequest = (OneTimeWorkRequest) OneTimeWorkRequest.Builder
                .From<OrderCheckerBackgroundWorker>()
                .SetInitialDelay(15, TimeUnit.Minutes)!
                .AddTag(WorkNameTag)
                .Build();

            //workManager.Enqueue(workRequest);
            workManager.EnqueueUniqueWork(k_UniqueRescheduledWorkerName, ExistingWorkPolicy.Replace!, workRequest);
            Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "We are logged in, rescheduling self (15min)");

            return Result.InvokeSuccess();
        }

        //Login for checking session
        authService.EnsureAutoLoginStateLoadedAsync().GetAwaiter().GetResult(); 
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), $"Auto login setup: {authService.IsAutoLoginSetup}");

        if (!authService.IsAutoLoginSetup)
        {
            //Problem -> Auto login not setup
            //Unschedule self

            Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "Auto login not setup, un-scheduling work");
            WorkManager.GetInstance(ApplicationContext).CancelUniqueWork(UniqueWorkerName);

            return Result.InvokeFailure();
        }

        var loginResult = authService.AttemptAutoLoginAsync().GetAwaiter().GetResult();
        if (loginResult is not null)
        {
            //Problem -> Auto login failed
            //Unschedule self

            Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "Login through Auto Login failed, un-scheduling work");
            WorkManager.GetInstance(ApplicationContext).CancelUniqueWork(UniqueWorkerName);

            return Result.InvokeFailure();
        }

        //Check
        NotifyOnUnorderedDays();

        //Remove session
        authService.RemoveSessionAsync().GetAwaiter().GetResult();

        return AndroidX.Work.ListenableWorker.Result.InvokeSuccess();
    }

    private void NotifyOnUnorderedDays()
    {
        var orderCheckerService = IPlatformApplication.Current!.Services.GetService<OrderCheckerService>()!;
        var notificationManager = NotificationManagerCompat.From(ApplicationContext);

        var daysWithoutOrder = orderCheckerService.GetDaysWithoutOrder().GetAwaiter().GetResult();
        for (int i = 0; i < daysWithoutOrder.Count; i++)
        {
            Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), $"Day without order: {daysWithoutOrder[i].Date:dddd dd.MMMM}");

            DailyMenuDTO day = daysWithoutOrder[i];

            // Create notification using native android builder
            var builder = new NotificationCompat.Builder(ApplicationContext, "orders_notifications")
                .SetContentTitle("Neobjednáno")
                .SetContentText($"Nemáš objednáno na den {day.Date:dddd dd.MMMM}")
                .SetSmallIcon(ResourceConstant.Drawable.notificationicon)
                .SetAutoCancel(true);

            // Show
            var notification = builder.Build();
            notificationManager.Notify(i, notification);
        }
    }

        /// <summary>
    /// Schedules the OrderChecker background worker to run daily
    /// ensuring permissions and battery optimization
    /// </summary>
    /// <param name="initialDelayMillis">If null, use delay that syncs the task to run daily from time specified by the domain rules</param>
    public static async Task<bool> TryStartOrderCheckerAsync(long? initialDelayMillis = null)
    {
        var notificationPermissions = await Permissions.RequestAsync<Permissions.PostNotifications>();
        if (notificationPermissions != PermissionStatus.Granted)
            return false;

        //Request "android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS"
        //so that the background work can run in the background
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            var packageName = MainActivity.Instance.PackageName;
            var pm = MainActivity.Instance.GetSystemService(Context.PowerService) as PowerManager;

            MainActivity.Instance.OrderCheckerBatteryOptimizationResult = new TaskCompletionSource();

            if (!(pm!.IsIgnoringBatteryOptimizations(packageName)))
            {
                //Show request prompt and detect whether the user pressed accept or deny
                var intent = new Intent();
                intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                MainActivity.Instance.StartActivityForResult(intent, MainActivity.OrderCheckerBatteryRequestCode);

                //Wait until accept or deny
                await MainActivity.Instance.OrderCheckerBatteryOptimizationResult.Task;

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
    /// <param name="initialDelayMillis">
    /// If null, use delay that syncs the task to run daily
    /// from time specified by the domain rules: such that it runs everyday at 10:00 AM
    /// </param>
    private static void ScheduleOrderChecker(long? initialDelayMillis = null)
    {
        Android.Util.Log.Debug(nameof(MainActivity), "Starting Order Checker BW");
        var workManager = WorkManager.GetInstance(MainActivity.Instance);
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

    public static void StopOrderChecker()
    {
        Android.Util.Log.Debug(nameof(MainActivity), "Stopping Order Checker BW");
        var workManager = WorkManager.GetInstance(MainActivity.Instance);
        workManager.CancelAllWorkByTag(OrderCheckerBackgroundWorker.WorkNameTag); //Cancels rescheduled BWs as well
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>True if there is any order checker work scheduled/active</returns>
    public static bool GetIsOrderCheckerBackgroundWorkerScheduled()
    {
        WorkManager instance = WorkManager.GetInstance(MainActivity.Instance);
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
}
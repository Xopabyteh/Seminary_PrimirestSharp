using _Microsoft.Android.Resource.Designer;
using Android.Content;
using AndroidX.Core.App;
using AndroidX.Work;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;
using Android.Icu.Util;
using Calendar = Android.Icu.Util.Calendar;
using Locale = Java.Util.Locale;
using NetworkType = AndroidX.Work.NetworkType;
using TimeUnit = Java.Util.Concurrent.TimeUnit;
using TimeZone = Android.Icu.Util.TimeZone;
using Yearly.Contracts.Notifications;


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
            var builder = new NotificationCompat.Builder(ApplicationContext, PushContracts.General.k_GeneralNotificationChannelId)
                .SetContentTitle("Neobjednáno")
                .SetContentText($"Nemáš objednáno na den {day.Date:dddd dd.MMMM}")
                .SetSmallIcon(ResourceConstant.Drawable.notificationicon)
                .SetAutoCancel(true);

            // Show
            var notification = builder.Build();
            notificationManager.Notify(i + 233564647, notification);
        }
    }

    /// <summary>
    /// Schedules the OrderChecker background worker to run daily
    /// ensuring permissions and battery optimization
    /// </summary>
    /// <param name="initialDelayMillis">If null, use delay that syncs the task to run daily from time specified by the domain rules</param>
    public static async Task<bool> TryStart(long? initialDelayMillis = null)
    {
        var notificationPermissions = await Permissions.RequestAsync<Permissions.PostNotifications>();
        if (notificationPermissions != PermissionStatus.Granted)
            return false;

        if (!await MainActivity.Instance.RequestBatteryUnoptimizedIfNeeded())
            return false;

        //This version doesn't require such permissions or we have them already
        //Start and return true
        Schedule(initialDelayMillis);
        return true;
    }

    public static void StopOrderChecker()
    {
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "Stopping Order Checker BW");
        var workManager = WorkManager.GetInstance(MainActivity.Instance);
        workManager.CancelAllWorkByTag(OrderCheckerBackgroundWorker.WorkNameTag); //Cancels rescheduled BWs as well
    }

    /// <param name="initialDelayMillis">
    /// If null, use delay that syncs the task to run daily
    /// from time specified by the domain rules: such that it runs everyday at 10:00 AM
    /// </param>
    private static void Schedule(long? initialDelayMillis = null)
    {
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "Starting Order Checker BW");
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
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), $"Initial offset {initialOffsetMillis} millis");

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
            .AddTag(WorkNameTag)
            .SetInitialDelay(initialOffsetMillis, TimeUnit.Milliseconds!)!;

        var ocRequest = (PeriodicWorkRequest)ocBuilder.Build();

        workManager.EnqueueUniquePeriodicWork(
            UniqueWorkerName,
            ExistingPeriodicWorkPolicy.Keep!,
            ocRequest);
    }
}
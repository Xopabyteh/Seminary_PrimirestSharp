using _Microsoft.Android.Resource.Designer;
using Android.Content;
using AndroidX.Work;
using Yearly.MauiClient.Services;
using Android.Icu.Util;
using AndroidX.Core.App;
using Calendar = Android.Icu.Util.Calendar;
using Locale = Java.Util.Locale;
using NetworkType = AndroidX.Work.NetworkType;
using TimeUnit = Java.Util.Concurrent.TimeUnit;
using TimeZone = Android.Icu.Util.TimeZone;
using Yearly.Contracts.Notifications;


namespace Yearly.MauiClient;

public class BalanceCheckerBackgroundWorker : Worker
{
    public const string UniqueWorkerName = $"psharp-{nameof(BalanceCheckerBackgroundWorker)}";
    private const string k_UniqueRescheduledWorkerName = $"psharp-{nameof(BalanceCheckerBackgroundWorker)}-rescheduled";
    public const string WorkNameTag = $"psharp-{nameof(BalanceCheckerBackgroundWorker)}-tag";

    private const int k_minMoneyCzechCrownsThreshold = 200;

    public BalanceCheckerBackgroundWorker(Context context, WorkerParameters workerParams) 
        : base(context, workerParams)
    {
    }

    private async Task CheckAndNotifyAsync()
    {
        // Check
        //var minMoneyThreshold = Preferences.Get(SettingsPage.BalanceMinimumThresholdPrefKey, SettingsPage.BalanceMinimumThresholdDefault);
        
        var menuAndOrderCacheService = IPlatformApplication.Current!.Services.GetService<MenuAndOrderCacheService>()!;
        await menuAndOrderCacheService.EnsureBalanceLoadedAsync();
        var balance = menuAndOrderCacheService.GetBalanceDetails();

        var moneyLeft = balance.BalanceCrowns - balance.OrderedForCrowns;
        Android.Util.Log.Debug(nameof(BalanceCheckerBackgroundWorker), $"Money left: {moneyLeft} compared to threshold: {k_minMoneyCzechCrownsThreshold}. Balance - {balance.BalanceCrowns}; Ordered for - {balance.OrderedForCrowns}");
        if (moneyLeft > k_minMoneyCzechCrownsThreshold)
            return;
        
        //Notify
        // Create notification using native android builder
        var builder = new NotificationCompat.Builder(ApplicationContext, PushContracts.General.k_GeneralNotificationChannelId)
            .SetContentTitle("Málo love")
            .SetContentText($"Zbývá ti na účtě {moneyLeft:0} Kč")
            .SetSmallIcon(ResourceConstant.Drawable.notificationicon)
            .SetAutoCancel(true);

        // Show
        var notification = builder.Build();
        var notificationManager = NotificationManagerCompat.From(ApplicationContext);
        notificationManager.Notify(54654132, notification);
    }

    public override AndroidX.Work.ListenableWorker.Result DoWork()
    {
        //Log
        Android.Util.Log.Debug(nameof(BalanceCheckerBackgroundWorker), "DoWork");

        var authService = IPlatformApplication.Current!.Services.GetService<AuthService>()!;

        if (authService.IsLoggedIn)
        {
            //Don't try to trigger this, when viewing the app
            // 1. The user himself can see whether he has the balance himself
            // 2. It will mess up authentication

            //Reschedule self in 15 minutes
            var workManager = WorkManager.GetInstance(ApplicationContext);
            var workRequest = (OneTimeWorkRequest) OneTimeWorkRequest.Builder
                .From<BalanceCheckerBackgroundWorker>()
                .SetInitialDelay(15, TimeUnit.Minutes)!
                .AddTag(WorkNameTag)
                .Build();

            //workManager.Enqueue(workRequest);
            workManager.EnqueueUniqueWork(k_UniqueRescheduledWorkerName, ExistingWorkPolicy.Replace!, workRequest);
            Android.Util.Log.Debug(nameof(BalanceCheckerBackgroundWorker), "We are logged in, rescheduling self (15min)");

            return Result.InvokeSuccess();
        }

        //Login for checking session
        authService.EnsureAutoLoginStateLoadedAsync().GetAwaiter().GetResult(); 
        Android.Util.Log.Debug(nameof(BalanceCheckerBackgroundWorker), $"Auto login setup: {authService.IsAutoLoginSetup}");

        if (!authService.IsAutoLoginSetup)
        {
            //Problem -> Auto login not setup
            //Unschedule self

            Android.Util.Log.Debug(nameof(BalanceCheckerBackgroundWorker), "Auto login not setup, un-scheduling work");
            WorkManager.GetInstance(ApplicationContext).CancelUniqueWork(UniqueWorkerName);

            return Result.InvokeFailure();
        }

        var loginResult = authService.AttemptAutoLoginAsync().GetAwaiter().GetResult();
        if (loginResult is not null)
        {
            //Problem -> Auto login failed
            //Unschedule self

            Android.Util.Log.Debug(nameof(BalanceCheckerBackgroundWorker), "Login through Auto Login failed, un-scheduling work");
            WorkManager.GetInstance(ApplicationContext).CancelUniqueWork(UniqueWorkerName);

            return Result.InvokeFailure();
        }

        //Check
        CheckAndNotifyAsync().GetAwaiter().GetResult();

        //Remove session
        authService.RemoveSessionAsync().GetAwaiter().GetResult();

        return AndroidX.Work.ListenableWorker.Result.InvokeSuccess();
    }

    /// <summary>
    /// Schedules the background worker to run daily
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

    public static void Stop()
    {
        Android.Util.Log.Debug(nameof(MainActivity), "Stopping Balance Checker BW");
        var workManager = WorkManager.GetInstance(MainActivity.Instance);
        workManager.CancelAllWorkByTag(WorkNameTag); // Cancels rescheduled BWs as well
    }

    /// <param name="initialDelayMillis">
    /// If null, use delay that syncs the task to run daily
    /// from time specified by the domain rules: such that it runs everyday at 10:00 AM
    /// </param>
    private static void Schedule(long? initialDelayMillis = null)
    {
        var workManager = WorkManager.GetInstance(MainActivity.Instance);
        var czechLocale = new Locale("cs", "CZ");
        var czechTimezone = TimeZone.GetTimeZone("Europe/Prague");
        var currentDate = Calendar.GetInstance(czechTimezone, czechLocale)!;
        
        // Background Worker (Every day at 10:00 AM czech time)
        var ocDate = Calendar.GetInstance(czechTimezone, czechLocale)!;
        ocDate.Set(CalendarField.HourOfDay, 10);
        ocDate.Set(CalendarField.Minute, 0);
        ocDate.Set(CalendarField.Second, 0);
        if (ocDate.Before(currentDate)) //Move next execution for tomorrow, if it's too late for today
        {
            ocDate.Add(CalendarField.HourOfDay, 24);
        }
        var initialOffsetMillis = initialDelayMillis ?? ocDate.TimeInMillis - currentDate.TimeInMillis;

        var ocConstraints = new Constraints.Builder()
            .SetRequiredNetworkType(NetworkType.Connected!)
            .SetRequiresCharging(false)
            .SetRequiresBatteryNotLow(false)
            .SetRequiresDeviceIdle(false)
            .SetRequiresStorageNotLow(false)
            .Build();

        var ocBuilder = new PeriodicWorkRequest.Builder
                (typeof(BalanceCheckerBackgroundWorker), TimeSpan.FromDays(1))
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
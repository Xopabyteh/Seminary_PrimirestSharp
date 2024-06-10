using _Microsoft.Android.Resource.Designer;
using Android.Content;
using AndroidX.Core.App;
using AndroidX.Work;
using Java.Util.Concurrent;
using Yearly.Contracts.Menu;
using Yearly.MauiClient.Services;

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
}
using Android.Content;
using AndroidX.Work;
using Java.Util.Concurrent;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Yearly.MauiClient.Services;
using Yearly.MauiClient.Services.SharpApiFacades;

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

    public override Result DoWork()
    {
        //Log
        Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "DoWork");

        var authService = IPlatformApplication.Current!.Services.GetService<AuthService>()!;
        var authFacade = IPlatformApplication.Current!.Services.GetService<AuthenticationFacade>()!;

        if (authService.IsLoggedIn)
        {
            //Don't try to trigger this, when viewing the app
            // 1. The user himself can see whether he has ordered stuff already
            // 2. It will mess up authentication

            //Reschedule self in 15 minutes
            var workManager = WorkManager.GetInstance(ApplicationContext)!;
            var workRequest = (OneTimeWorkRequest) OneTimeWorkRequest.Builder
                .From<OrderCheckerBackgroundWorker>()
                //.SetInitialDelay(15, TimeUnit.Minutes)!
                .SetInitialDelay(1, TimeUnit.Minutes)! //Todo: change back to 15
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
            WorkManager.GetInstance(ApplicationContext)!.CancelUniqueWork(UniqueWorkerName);

            return Result.InvokeFailure();
        }

        var loginResult = authFacade.LoginAsync(authService.AutoLoginStoredCredentials!).GetAwaiter().GetResult();
        if (loginResult.IsT1)
        {
            //Problem -> Auto login failed
            //Unschedule self

            Android.Util.Log.Debug(nameof(OrderCheckerBackgroundWorker), "Login through Auto Login failed, un-scheduling work");
            WorkManager.GetInstance(ApplicationContext)!.CancelUniqueWork(UniqueWorkerName);

            return Result.InvokeFailure();
        }

        var loginResponse = loginResult.AsT0;
        authService.SetSessionAsync(loginResponse).GetAwaiter().GetResult();

        //Check
        CheckIfHasOrdered().GetAwaiter().GetResult();

        //Remove session
        authService.RemoveSessionAsync().GetAwaiter().GetResult();

        return Result.InvokeSuccess();
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
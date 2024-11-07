using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using Java.Lang;
using Java.Util.Concurrent;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.Core.Platforms.Android;
using Yearly.MauiClient.Components.Layout;
using Yearly.MauiClient.Services;

namespace Yearly.MauiClient;

[Activity(
    LaunchMode = LaunchMode.SingleTop,
    Theme = "@style/PSharpSplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static MainActivity Instance { get; private set; } = null!;

    private const int k_BatteryOptimizationRequestCode = 137; //Random number
    private TaskCompletionSource? batteryOptimizationResult;

    public MainActivity()
    {
        Instance = this;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        //Xamarin essentials
        Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle

        CrossFirebase.Initialize(this);
        FirebaseCloudMessagingImplementation.OnNewIntent(this.Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        
        base.OnNewIntent(intent);
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
            case k_BatteryOptimizationRequestCode:
                if (batteryOptimizationResult is not null)
                {
                    batteryOptimizationResult.SetResult();
                    batteryOptimizationResult = null;
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

    public async Task<bool> RequestBatteryUnoptimizedIfNeeded()
    {
        //Request "android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS"
        //so that the background work can run in the background
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            var packageName = PackageName;
            var pm = GetSystemService(Context.PowerService) as PowerManager;

            batteryOptimizationResult = new TaskCompletionSource();

            if (!(pm!.IsIgnoringBatteryOptimizations(packageName)))
            {
                //Show request prompt and detect whether the user pressed accept or deny
                var intent = new Intent();
                intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                StartActivityForResult(intent, k_BatteryOptimizationRequestCode);

                //Wait until accept or deny
                await batteryOptimizationResult.Task;

                //Check if the user accepted
                if (!(pm.IsIgnoringBatteryOptimizations(packageName)))
                {
                    //Don't start and return false...
                    return false;
                }
            }
        }

        return true;
    }

    /// <returns>True if there is any work scheduled/active with provided tag</returns>
    public bool GetIsBackgroundWorkerScheduled(string workTagName)
    {
        WorkManager instance = WorkManager.GetInstance(this);
        IListenableFuture statuses = instance.GetWorkInfosByTag(workTagName);
        try
        {
            var running = false;
            dynamic workInfoList = statuses.Get()!;
            if (workInfoList == null)
                return false;

            foreach (WorkInfo workInfo in workInfoList)
            {
                WorkInfo.State state = workInfo.GetState();
                running = state == WorkInfo.State.Running! | state == WorkInfo.State.Enqueued;
            }
            return running;
        }
        catch (ExecutionException e)
        {
            Log.Debug(nameof(MainActivity), "Error getting work info - stack trace: {0}", e.StackTrace);
            return false;
        }
        catch (InterruptedException e)
        {
            Log.Debug(nameof(MainActivity), "Error getting work info - stack trace: {0}", e.StackTrace);
            return false;
        }
    }
}
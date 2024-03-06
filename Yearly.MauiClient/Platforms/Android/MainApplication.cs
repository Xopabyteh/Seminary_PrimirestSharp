using Android.App;
using Android.Runtime;

namespace Yearly.MauiClient
{
#if DEBUG
    [Application(UsesCleartextTraffic = true)]
#else
    [Application]
#endif
    [IntentFilter(
        new[] {
            Shiny.ShinyPushIntents.NotificationClickAction
        },
        Categories = new[] {
            "android.intent.category.DEFAULT"
        }
    )]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}

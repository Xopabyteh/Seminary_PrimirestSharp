using Android.App;
using Android.Content;
using Android.Widget;
using AndroidX.Core.Content;

namespace Yearly.MauiClient;

[BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
[IntentFilter(new[] { Intent.ActionBootCompleted })]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if(intent.Action != Intent.ActionBootCompleted)
            return;

        var t = Toast.MakeText(context, "Boot completed received!", ToastLength.Long)!;
        t.Show();

        //Start OrderCheckerForegroundService
        var serviceIntent = new Intent(context, typeof(OrderCheckerBackgroundService));
        ContextCompat.StartForegroundService(context, serviceIntent);
    }
}
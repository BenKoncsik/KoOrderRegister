using Android.App;
using Android.Runtime;

namespace KoOrderRegister
{
    [Application]
    public class MainApplication : MauiApplication
    {
        // Minimum permissions 
        [assembly: UsesPermission(Manifest.Permission.Vibrate)]
        [assembly: UsesPermission("android.permission.POST_NOTIFICATIONS")]

        // Required so that the plugin can schedule
        [assembly: UsesPermission(Manifest.Permission.WakeLock)]

        //Required so that the plugin can reschedule notifications upon a reboot
        [assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]


        //Optional (only for Calendar and alarm clock apps)
        //If the app requires scheduling notifications with exact timings (aka exact alarms), there are two options since Android 14 brought about behavioural changes

        // Users will not be prompted to grant permission, however as per the official Android documentation on the USE_EXACT_ALARM permission
        // (refer to (https://developer.android.com/about/versions/14/changes/schedule-exact-alarms#calendar-alarm-clock) and (https://developer.android.com/reference/android/Manifest.permission#USE_EXACT_ALARM)),
        // this requires the app to target Android 13 (API level 33) or higher and could be subject to approval and auditing by the app store(s) used to publish theapp
        [assembly: UsesPermission("android.permission.USE_EXACT_ALARM")]

        // user can grant the permission via the app
        [assembly: UsesPermission("android.permission.SCHEDULE_EXACT_ALARM")]
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}

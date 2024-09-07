using Android.App;
using Android.Content.PM;
using Android.OS;
using KoOrderRegister.Platforms.Android.Utils;

namespace KoOrderRegister
{
    
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static string AppVersion { get; private set; } = string.Empty;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppVersion = VersionUtils.GetAppVersion(this);
            base.OnCreate(savedInstanceState);
        }
    }
}

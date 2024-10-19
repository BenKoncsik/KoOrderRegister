using Android.Content;
using Android.Content.PM;
using Java.Lang;
using System.Diagnostics;

namespace KoOrderRegister.Platforms.Android.Utils
{
    public static class VersionUtils {
        public static string GetAppVersion(Context context)
        {
            try
            {
                PackageInfo pInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
                return pInfo.VersionName;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }

    }
}

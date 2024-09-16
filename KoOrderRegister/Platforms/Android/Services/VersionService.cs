using KoOrderRegister.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Platforms.Android.Services
{
    public class VersionService: IVersionService
    {
        public string AppVersion => MainActivity.AppVersion;

        public string DeviceType => "android";

        public string UpdatePackageName => "KOR_update.apk";
    }
}

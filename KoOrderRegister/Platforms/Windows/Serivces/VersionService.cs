using KoOrderRegister.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace KoOrderRegister.Platforms.Windows.Services
{
    public class VersionService : IVersionService
    {
        public string AppVersion => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";

        public string DeviceType => RuntimeInformation.ProcessArchitecture.ToString();

        public string UpdatePackageName => "KOR_update.msix";
    }
}

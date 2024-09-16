using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Services
{
    public struct AppUpdateInfo
    {
        public string OldVersion { get; set; }
        public string NewVersion { get; set; }
        public string DownloadUrl { get; set; }
    }
    public interface IAppUpdateService
    {
        Task<AppUpdateInfo> CheckForAppInstallerUpdatesAndLaunchAsync();
        Task<string> DownloadFileAsync(string fileUrl, IProgress<double> progress);
        string AppVersion { get; }
    }
}

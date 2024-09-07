using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Services
{
    public class PlaceHolderAppUpdateService : IAppUpdateService
    {
        public async Task<AppUpdateInfo> CheckForAppInstallerUpdatesAndLaunchAsync()
        {
            return new AppUpdateInfo();
        }
        public async Task<string> DownloadFileAsync(string fileUrl, IProgress<double> progress)
        {
            return string.Empty;
        }


    }
}

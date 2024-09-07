using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Services
{
    public interface IAppUpdateService
    {
        Task DownloadDialog(string updateUrl, string oldVersion, string newVersion);
        void CheckForAppInstallerUpdatesAndLaunchAsync();
    }
}

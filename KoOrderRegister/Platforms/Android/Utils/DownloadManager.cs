using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager
{
    static partial class DownloadManager
    {
        static string PlatformFolder() =>
            Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path;
    }
}

using System;
using System.IO;

namespace DownloadManager
{
    static partial class DownloadManager
    {
        static string PlatformFolder()
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(docsPath, "..", "Library");
        }
    }
}
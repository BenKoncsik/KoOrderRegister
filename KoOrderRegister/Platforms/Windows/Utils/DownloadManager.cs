using Windows.Storage;

namespace DownloadManager
{
    static partial class DownloadManager
    {
        static string PlatformFolder()
        {
            return FileSystem.CacheDirectory;
           
        }
    }
}
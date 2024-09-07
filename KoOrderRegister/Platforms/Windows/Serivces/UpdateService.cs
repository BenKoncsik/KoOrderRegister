using KoOrderRegister.Localization;
using KoOrderRegister.Services;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace KoOrderRegister.Platforms.Windows.Service
{
    public class UpdateService : IAppUpdateService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiUrl = "https://api.github.com/repos/BenKoncsik/KoOrderRegister/releases/latest";
        private static DateTime _lastUpdateCheck = DateTime.MinValue;
        public async Task<AppUpdateInfo> CheckForAppInstallerUpdatesAndLaunchAsync()
        {
            try
            {
                var (latestVersion, msixUrl) = await GetLatestReleaseInfoAsync();
                if (latestVersion == null) return new AppUpdateInfo();

                var currentVersion = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
                if (new Version(latestVersion) > new Version(currentVersion))
                {
                    return  new AppUpdateInfo
                    {
                        OldVersion = currentVersion,
                        NewVersion = latestVersion,
                        DownloadUrl = msixUrl
                    };
                    
                }
#if DEBUG
                else
                {
                    return new AppUpdateInfo
                    {
                        OldVersion = currentVersion,
                        NewVersion = latestVersion,
                        DownloadUrl = msixUrl
                    };
                }
#endif
                return new AppUpdateInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new AppUpdateInfo();
            }

        }


        public async Task<(string version, string msixUrl)> GetLatestReleaseInfoAsync()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "YourApp");

            var response = await _httpClient.GetAsync(_apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var release = JsonConvert.DeserializeObject<GitHubRelease>(jsonResponse);
                string msixUrl = string.Empty;
                string version = string.Empty;
                // Check for the correct architecture
                var architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
                foreach (var asset in release.Assets)
                {
                    if (asset.name.ToLower().Contains(architecture.ToLower()))
                    {
                        msixUrl = asset.browser_download_url;
                        version = asset.browser_download_url.Split("/").Last().Split("_")[1];
                        break;
                    }

                }

                return (version, msixUrl);
            }

            return (null, null);
        }

        private class GitHubRelease
        {
            public string TagName { get; set; }
            public Asset[] Assets { get; set; }

            public class Asset
            {
                public string browser_download_url { get; set; }
                public string name { get; set; }
            }
        }

        public async Task<string> DownloadFileAsync(string fileUrl, IProgress<double> progress)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error {response.StatusCode} downloading file.");
            }

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var readBytes = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            string localPath = Path.Combine(FileSystem.CacheDirectory, "KORUpdate.msix");

            using (var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                do
                {
                    var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer, 0, read);

                        readBytes += read;
                        progress.Report((readBytes / (double)totalBytes) * 100);
                    }
                }
                while (isMoreToRead);
            }

            return localPath;
        }

    }
    }


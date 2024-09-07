using KoOrderRegister.Localization;
using KoOrderRegister.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace KoOrderRegister.Platforms.Android.Service
{
    public class UpdateService : IAppUpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://api.github.com/repos/BenKoncsik/KoOrderRegister/releases/latest";
        private static DateTime _lastUpdateCheck = DateTime.MinValue;
        
        public UpdateService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GitHubClient");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourApp");
        }
        
        public async Task<AppUpdateInfo> CheckForAppInstallerUpdatesAndLaunchAsync()
        {
            try
            {
                var (latestVersion, msixUrl) = await GetLatestReleaseInfoAsync();
                if (latestVersion == null) return new AppUpdateInfo();

                var currentVersion = MainActivity.AppVersion;
                if (new Version(latestVersion) > new Version(currentVersion))
                {
                    return new AppUpdateInfo
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
            var response = await _httpClient.GetAsync(_apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var release = JsonConvert.DeserializeObject<GitHubRelease>(jsonResponse);
                string msixUrl = string.Empty;
                string version = string.Empty;
                // Check for the correct architecture
                var architecture = "android";
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
            var httpClient = new HttpClient(); // Consider reusing HttpClient instances or using IHttpClientFactory
            try
            {
                var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error {response.StatusCode} downloading file.");
                }

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var readBytes = 0L;
                var buffer = new byte[8192];

                string localPath = Path.Combine(FileSystem.CacheDirectory, "KOR_update.apk");

                using (var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true))
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        int read;
                        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            readBytes += read;
                            progress.Report((double)readBytes / totalBytes * 100);
                        }
                    }
                }

                return localPath;
            }
            finally
            {
                httpClient.Dispose();
            }
        }


    }
}


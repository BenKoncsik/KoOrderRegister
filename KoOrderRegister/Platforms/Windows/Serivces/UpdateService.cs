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
            DownloadManager.DownloadManager.UseCustomHttpClient(_httpClient);
            return await DownloadManager.DownloadManager.DownloadAsync("KOR_update.msix", fileUrl, progress);
        }

    }
    }


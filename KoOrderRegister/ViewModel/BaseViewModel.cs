using KoOrderRegister.Localization;
using KoOrderRegister.Services;
using KoOrderRegister.Utility;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private readonly IAppUpdateService _updateService;
        private readonly ILocalNotificationService _notifyService;
        #region Binding varrible
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (value != _isRefreshing)
                {
                    _isRefreshing = value;
                    OnPropertyChanged(nameof(IsRefreshing));
                }
            }
        }
        private string _loadingTXT = AppRes.Loading;
        public string LoadingTXT
        {
            get => _loadingTXT;
            set
            {
                if (value != _loadingTXT)
                {
                    _loadingTXT = value;
                    OnPropertyChanged(nameof(LoadingTXT));
                }
            }
        }
        private static bool _isRun = false;
        private static bool _isDownloading = false;
        private int notificationId = -1;
        private string filePath = string.Empty;

#if DEBUG
        public bool IsBetaFunctions { get; set; } = true;
#elif DEVBUILD
        public bool IsBetaFunctions { get; set; } = true;
#else
        public bool IsBetaFunctions { get; set; } = false;
#endif


        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (TargetInvocationException ex)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException}");
            }

        }

        public BaseViewModel(IAppUpdateService updateService, ILocalNotificationService notificationService) : this()
        {
            _updateService = updateService;
            _notifyService = notificationService;
            _notifyService.NotificationReceived += Current_NotificationActionTapped;
            if (!_isRun)
            {
                _isRun = true;
                OnStart();
            }
        }

        public BaseViewModel()
        {
            settUserTheme();
        }
        #region AppUpdate method
        private async Task OnStart()
        {
            if(_updateService == null)
            {
                return;
            }
            try
            {
                await CheckUpdateInBackground();
                // Check for update every 1 hour
                System.Timers.Timer timer = new System.Timers.Timer(3600000);
                timer.Enabled = true;
                timer.Elapsed += async (sender, e) => await CheckUpdateInBackground();
                timer.AutoReset = true;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        private async Task CheckUpdateInBackground()
        {
            if (!_isDownloading)
            {
                AppUpdateInfo info = await _updateService.CheckForAppInstallerUpdatesAndLaunchAsync();
                Version oldVersion = new Version(info.OldVersion);
                Version newVersion = new Version(info.NewVersion);
                if (!string.IsNullOrEmpty(info.NewVersion) && !string.IsNullOrEmpty(info.DownloadUrl) &&
                    newVersion > oldVersion)
                {
                    await ShowUpdateDialog();
                }
            }
        }

        protected async Task CheckUpdate()
        {
            if (!_isDownloading)
            {
                await ShowUpdateDialog();
            }
            
        }
        
        private AndroidOptions android = new AndroidOptions
        {
            ChannelId = "kor_general",
            IconSmallName =
            {
                ResourceName = "appicon.png",
            },
            Ongoing = true,
            ProgressBar = new AndroidProgressBar
            {
                IsIndeterminate = false,
                Max = 100,
                Progress = 0,
            }
        };
        private async Task ShowUpdateDialog()
        {
            AppUpdateInfo info = await _updateService.CheckForAppInstallerUpdatesAndLaunchAsync();
            if (string.IsNullOrEmpty(info.NewVersion) || string.IsNullOrEmpty(info.DownloadUrl))
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.UpdateApp, AppRes.NoNewVersion, AppRes.Ok);
                return;
            }
            bool result = false;
            Device.BeginInvokeOnMainThread(async () =>
            {
                result = await Application.Current.MainPage.DisplayAlert(AppRes.UpdateApp,
                $"{AppRes.NewVersionAvailable}: {info.OldVersion} --> {info.NewVersion}",
                AppRes.Ok, AppRes.No);
                _isDownloading = true;
                if (result)
                {
                    ThreadManager.Run(async () => await startDownload());
                }
            });

            async Task startDownload()
            {
                LoadingTXT = AppRes.Downloading;
                notificationId = _notifyService.SendNotification(AppRes.UpdateApp, AppRes.Update);
                filePath = await _updateService.DownloadFileAsync(info.DownloadUrl, new Progress<double>(progress =>
                {
                    Console.WriteLine($"Downloaded {progress}%");
                    LoadingTXT = $"{AppRes.Downloading}: {Math.Round(progress, 2)}%";
                    android.ProgressBar.Progress = (int)progress;
                    _notifyService.UpdateNotification(notificationId, AppRes.Downloading, LoadingTXT, NotificationCategoryType.None, android);
                    if (progress >= 99.99)
                    {
                        _notifyService.DeleteNotification(notificationId);
                        notificationId = _notifyService.SendNotification(AppRes.Downloading, AppRes.Done);
                    }
                }));
                _isDownloading = false;
            }
        }
        private async void Current_NotificationActionTapped(NotificationActionArgs e)
        {
            if (e.Id == notificationId && !string.IsNullOrEmpty(filePath))
            {
                if (await Application.Current.MainPage.DisplayAlert(AppRes.UpdateApp, AppRes.UpdateDownloaded, AppRes.Open, AppRes.Cancel))
                {
                    Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
                }
            }
        }

        #endregion

        #region App user theme
        private static bool _isThemeSet = false;
        private void settUserTheme()
        {
            if(_isThemeSet)
            {
                return;
            }
            _isThemeSet = true;
            bool AutomaticUserTheme = Preferences.Get("IsThemeAutomatic", true);
            if (!AutomaticUserTheme)
            {
                string UserTheme = Preferences.Get("UserTheme", "Light");
                App.Current.UserAppTheme = UserTheme switch
                {
                    "Light" => AppTheme.Light,
                    "Dark" => AppTheme.Dark,
                    _ => AppTheme.Light
                };
            }
        }
         #endregion
    }
}

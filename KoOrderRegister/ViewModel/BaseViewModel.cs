﻿using KoOrderRegister.Localization;
using KoOrderRegister.Services;
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

        public BaseViewModel(IAppUpdateService updateService)
        {
            _updateService = updateService;
            if (!_isRun)
            {
                _isRun = true;
                OnStart();
            }
        }
        public BaseViewModel()
        {

        }
        
        private async void OnStart()
        {
            if(_updateService == null)
            {
                return;
            }
            try
            {
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
            AppUpdateInfo info = await _updateService.CheckForAppInstallerUpdatesAndLaunchAsync();
            Version oldVersion = new Version(info.OldVersion);
            Version newVersion = new Version(info.NewVersion);
            if ((string.IsNullOrEmpty(info.NewVersion) || string.IsNullOrEmpty(info.DownloadUrl)) &&
                newVersion > oldVersion)
            {
                await ShowUpdateDialog();
            }
        }

        protected async void CheckUpdate()
        {
            await ShowUpdateDialog();
        }

        private async Task ShowUpdateDialog()
        {
            AppUpdateInfo info = await _updateService.CheckForAppInstallerUpdatesAndLaunchAsync();
            if (string.IsNullOrEmpty(info.NewVersion) || string.IsNullOrEmpty(info.DownloadUrl))
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.UpdateApp, AppRes.NoNewVersion, AppRes.Ok);
                return;
            }
            if (await Application.Current.MainPage.DisplayAlert(AppRes.UpdateApp,
                $"{AppRes.NewVersionAvailable}: {info.OldVersion} --> {info.NewVersion}",
                AppRes.Ok, AppRes.No))
            {
                LoadingTXT = AppRes.Downloading;
                IsRefreshing = true;
                string filePath = await _updateService.DownloadFileAsync(info.DownloadUrl, new Progress<double>(progress =>
                {
                    Console.WriteLine($"Downloaded {progress}%");
                    LoadingTXT = $"{AppRes.Downloading}: {Math.Round(progress, 2)}%";
                }));
                IsRefreshing = false;
                LoadingTXT = AppRes.Loading;
                if (await Application.Current.MainPage.DisplayAlert(AppRes.UpdateApp, AppRes.UpdateDownloaded, AppRes.Open, AppRes.Cancle))
                {
                    await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
                }
            }
        }
    }
}
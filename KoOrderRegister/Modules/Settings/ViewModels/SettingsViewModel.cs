using CommunityToolkit.Maui.Storage;
using DocumentFormat.OpenXml.Presentation;
using KoOrderRegister.Localization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Services;
using KoOrderRegister.Utility;
using KoOrderRegister.ViewModel;
using KORCore.Modules.Database.Factory;
using KORCore.Modules.Database.Services;
using KORCore.Utility;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;



namespace KoOrderRegister.Modules.Settings.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IDatabaseModel _databaseModel;
        private readonly IAppUpdateService _updateService;
        private readonly ILocalNotificationService _notificationService;


        public ObservableCollection<ILanguageSettings> LanguageSettings => new ObservableCollection<ILanguageSettings>(LanguageManager.LanguageSettingsInstances);
        private ILanguageSettings _selectedItem;
        public ILanguageSettings SelectedItem
        {
            get
            {
                if (_selectedItem == null)
                {
                    _selectedItem = LanguageManager.GetCurrentLanguage();
                }
                return _selectedItem;
            }

            set
            {
                if (value != null && !value.Equals(_selectedItem))
                {
                    _selectedItem = value;
                    ChangeLanguage(value);
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        private bool _isAutomaticTheme = Preferences.Get("IsThemeAutomatic", true);
        public bool IsAutoUserTheme
        {
            get => _isAutomaticTheme;
            set
            {
                if (value != _isAutomaticTheme)
                {
                    _isAutomaticTheme = value;
                    SettAutomaticUserTheme(value);
                    OnPropertyChanged(nameof(IsAutoUserTheme));
                }
            }
        }


        #region Commands
        public ICommand BackUpDatabaseCommand => new Microsoft.Maui.Controls.Command(BackUp);
        public ICommand RestoreDatabaseCommand => new Microsoft.Maui.Controls.Command(Restore);
        public ICommand AppUpdateCommand => new Microsoft.Maui.Controls.Command(UpdateApp);
        public ICommand AppThemeSwitchCommand => new Microsoft.Maui.Controls.Command(SwitchUserTheme);
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        
        
        public SettingsViewModel(LocalDatabaseModel databaseModel, IAppUpdateService updateService, ILocalNotificationService notificationService) : base(updateService, notificationService)
        {
            _databaseModel = databaseModel;
            _updateService = updateService;
            _notificationService = notificationService;
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
        private int notificationId = -1;
        public async void BackUp()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            try
            {
                var result = await FolderPicker.PickAsync(new CancellationToken());
                if (result != null && result.IsSuccessful && !string.IsNullOrEmpty(result.Folder.Path))
                {
                    var fileName = "koBackup.kncsk";
                    var fullPath = Path.Combine(result.Folder.Path, fileName);
                    notificationId = _notificationService.SendNotification(AppRes.BackupDatabase, AppRes.BackupDatabase, NotificationCategoryType.None, android);
                    await _databaseModel.ExportDatabaseToJson(fullPath, cancellationToken.Token, ProgressCallback);
                }
            }
            finally
            {
                cancellationToken.Cancel();
                cancellationToken.Dispose();
            }
            
        }   
        
        public async void Restore()
        {
            if(await Application.Current.MainPage.DisplayAlert(AppRes.RestoreDatabase, AppRes.AreYouSureYouRestore, AppRes.Ok, AppRes.No))
            {
                try
                {
                    var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.item" } }, 
                        { DevicePlatform.Android, new[] { "*/*" } }, 
                        { DevicePlatform.WinUI, new[] { ".kncsk" } }, 
                        { DevicePlatform.MacCatalyst, new[] { "public.item" } } 
                    });
                    var pickResult = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = AppRes.PlsSelectBackupFile,
                        FileTypes = customFileType
                    });
                    if (pickResult != null)
                    {
                        IsRefreshing = true;
                        Stream streamResult = await pickResult.OpenReadAsync();
                        if (streamResult != null)
                        {
                            await _databaseModel.ImportDatabaseFromJson(streamResult, ProgressCallback);
                            IsRefreshing = false;
                            await Application.Current.MainPage.DisplayAlert(AppRes.RestoreDatabase, AppRes.DatabaseRestoredSuccessfully, AppRes.Ok);
                        }
                        else
                        {
                            IsRefreshing = false;
                            await Application.Current.MainPage.DisplayAlert(AppRes.RestoreDatabase, AppRes.FaliedToRestore, AppRes.Ok);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error picking file: {ex.Message}");
                    IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert(AppRes.RestoreDatabase, AppRes.FaliedToRestore, AppRes.Ok);
                }
                IsRefreshing = false;

            }
        }

        public void ProgressCallback(float precent)
        {
            LoadingTXT = $"{AppRes.Loading}: {precent}%";
            android.ProgressBar.Progress = (int)precent;
            _notificationService.UpdateNotification(notificationId, AppRes.BackupDatabase, LoadingTXT, NotificationCategoryType.None, android);
            if(precent >= 100)
            {
                _notificationService.UpdateNotification(notificationId, AppRes.BackupDatabase, AppRes.Done);
            }
        }

        public void ChangeLanguage(ILanguageSettings languageSettings)
        {
            LanguageManager.SetLanguage(languageSettings);
        }

        public void SwitchUserTheme()
        {
            IsAutoUserTheme = false;
            if (Application.Current.UserAppTheme == AppTheme.Light)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
                Preferences.Set("UserTheme", AppTheme.Dark.ToString());
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
                Preferences.Set("UserTheme", AppTheme.Light.ToString());
            }
        }

        public void SettAutomaticUserTheme(bool isAutomatic)
        {
            Preferences.Set("IsThemeAutomatic", isAutomatic);
            if (isAutomatic)
            {
                Application.Current.UserAppTheme = Application.Current.PlatformAppTheme;
            }
        }

        public async void UpdateApp()
        {
            await ThreadManager.Run(async () => { await CheckUpdate(); }, ThreadManager.Priority.Low);
            
        }
    }
}

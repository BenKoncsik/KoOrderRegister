using CommunityToolkit.Maui.Storage;
using KoOrderRegister.Localization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.Export.Excel.Services;
using KoOrderRegister.Modules.Export.Services;
using KoOrderRegister.Services;
using KoOrderRegister.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ICommand BackUpDatabaseCommand => new Command(BackUp);
        public ICommand RestoreDatabaseCommand => new Command(Restore);
        public ICommand AppUpdateCommand => new Command(UpdateApp);
        public ICommand AppThemeSwitchCommand => new Command(SwitchUserTheme);
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        
        
        public SettingsViewModel(IDatabaseModel databaseModel, IAppUpdateService updateService, IExcelExportService exportService) : base(updateService)
        {
            _databaseModel = databaseModel;
            _updateService = updateService;
            _exportService = exportService;
        }

        public async void BackUp()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            try
            {
                var result = await FolderPicker.PickAsync(new CancellationToken());
                if (result != null && result.IsSuccessful && !string.IsNullOrEmpty(result.Folder.Path))
                {
                    IsRefreshing = true;
                    var fileName = "koBackup.kncsk";
                    var fullPath = Path.Combine(result.Folder.Path, fileName);
                    await _databaseModel.ExportDatabaseToJson(fullPath, cancellationToken.Token, ProgressCallback);
                    IsRefreshing = false;
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
                    Console.WriteLine($"Error picking file: {ex.Message}");
                    IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert(AppRes.RestoreDatabase, AppRes.FaliedToRestore, AppRes.Ok);
                }
                IsRefreshing = false;

            }
        }

        public void ProgressCallback(float precent)
        {
            LoadingTXT = $"{AppRes.Loading}: {precent}%";
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
            CheckUpdate();
        }



        #region BetaFunctions
        private readonly IExcelExportService _exportService;
        public ICommand ExportData => new Command(ExportDataToExcel);

        public async void ExportDataToExcel()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            try
            {
                var result = await FolderPicker.PickAsync(cancellationToken.Token);
                if (result != null && result.IsSuccessful && !string.IsNullOrEmpty(result.Folder.Path))
                {
                    IsRefreshing = true;
                    var fullPath = Path.Combine(result.Folder.Path);
                    await _exportService.Export(fullPath, cancellationToken.Token, ProgressCallback);
                    _exportService.CreateZip();
                    IsRefreshing = false;
                }
            }
            finally
            {
                cancellationToken.Cancel();
                cancellationToken.Dispose();
            }
        }
        #endregion
        }
}

using CommunityToolkit.Maui.Storage;
using KoOrderRegister.Localization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Services;
using KoOrderRegister.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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
        #region Commands
        public ICommand BackUpDatabaseCommand => new Command(BackUp);
        public ICommand RestoreDatabaseCommand => new Command(Restore);
        public ICommand AppUpdateCommand => new Command(UpdateApp);
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public SettingsViewModel(IDatabaseModel databaseModel, IAppUpdateService updateService) : base(updateService)
        {
            _databaseModel = databaseModel;
            _updateService = updateService;
        }

        public async void BackUp()
        {
            var result = await FolderPicker.PickAsync(new CancellationToken());
            if (result != null && result.IsSuccessful && !string.IsNullOrEmpty(result.Folder.Path))
            {
                IsRefreshing = true;
                string jsonContent = await _databaseModel.ExportDatabaseToJson();
                var fileName = "koBackup.kncsk";
                var fullPath = Path.Combine(result.Folder.Path, fileName);
                //File.WriteAllText(fullPath, jsonContent);
                IsRefreshing = false;
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
                        var jsonData = await File.ReadAllTextAsync(pickResult.FullPath);
                        if (!string.IsNullOrEmpty(jsonData))
                        {
                            await _databaseModel.ImportDatabaseFromJson(jsonData);
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

        public void ChangeLanguage(ILanguageSettings languageSettings)
        {
            LanguageManager.SetLanguage(languageSettings);
        }

        public async void UpdateApp()
        {
            CheckUpdate();
        }
    }
}

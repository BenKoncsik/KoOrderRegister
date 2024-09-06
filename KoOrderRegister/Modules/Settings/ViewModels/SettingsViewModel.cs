using KoOrderRegister.Localization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Modules.Database.Services;
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
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseModel _databaseModel;
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
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public SettingsViewModel(IDatabaseModel databaseModel)
        {
            _databaseModel = databaseModel;
        }

        public async void BackUp()
        {
            
            var filePickerOptions = new PickOptions
            {
                PickerTitle = AppRes.PlsChoosLocatoinToBackup,
            };

            var result = await FilePicker.PickAsync(filePickerOptions);
            if (result != null)
            {
                ActivityIndicator activityIndicator = new ActivityIndicator { IsRunning = true };

                string jsonContent = await _databaseModel.ExportDatabaseToJson();
                var fileName = "koBackup.kncsk";
                var fullPath = Path.Combine(result.FullPath, fileName);
                File.WriteAllText(fullPath, jsonContent);
                activityIndicator.IsRunning = false;


            }
        }   
        
        public async void Restore()
        {

        }

        public void ChangeLanguage(ILanguageSettings languageSettings)
        {
            LanguageManager.SetLanguage(languageSettings);
        }
    }
}

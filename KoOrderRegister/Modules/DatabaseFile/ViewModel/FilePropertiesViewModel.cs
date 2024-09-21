using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.ViewModel;
using Microsoft.Maui.Controls;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;



namespace KoOrderRegister.Modules.DatabaseFile.ViewModel
{
    public class FilePropertiesViewModel : BaseViewModel
    {
        #region DI
        private readonly IDatabaseModel _database;
        #endregion
        #region Binding varrible
        private FileModel file;
        public FileModel File
        {
            get => file;
            set
            {
                if (value != file)
                {
                    file = value;
                    OnPropertyChanged(nameof(File));
                    SettFileSize();
                }
            }
        }

        private bool _isAdvancedDetails = false;
        public bool IsAdvancedDetails
        {
            get => _isAdvancedDetails;
            set
            {
                if (value != _isAdvancedDetails)
                {
                    _isAdvancedDetails = value;
                    AdvancedDetailsTXT = value? AppRes.CloseAdvancedDetails : AppRes.OpenAdvancedDetails;
                    OnPropertyChanged(nameof(IsAdvancedDetails));
                }
            }
        }
        private string _advancedDetailsTXT = AppRes.OpenAdvancedDetails;
        public string AdvancedDetailsTXT
        {
            get => _advancedDetailsTXT;
            set
            {
                if (!value.Equals(_advancedDetailsTXT))
                {
                    _advancedDetailsTXT = value;
                    OnPropertyChanged(nameof(AdvancedDetailsTXT));
                }
            }
        }

        private string _fileSize = "0 B";
        public string FileSize
        {
            get => _fileSize;
            set
            {
                if (!value.Equals(_fileSize))
                {
                    _fileSize = value;
                    OnPropertyChanged(nameof(FileSize));
                }
            }
        }
        #endregion
        #region Command
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand OpenCloseAdvancedDetailsCommand { get; set; }
        #endregion

        public FilePropertiesViewModel(IDatabaseModel databaseModel)
        {
            _database = databaseModel;
            SaveCommand = new Command(Save);
            CancelCommand = new Command(Return);
            DeleteCommand = new Command(Delete);
            OpenCloseAdvancedDetailsCommand = new Command(OpenCloseAdvancedDetails);
        }

        public async void Save()
        {
            if(await _database.UpdateFile(File) > 0)
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.SuccessToSave + " " + File.Name, AppRes.Ok);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + File.Name, AppRes.Ok);
            }
        }   

        public async void Delete()
        {
            if (await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + File.Name, AppRes.No, AppRes.Yes))
            {
                IsRefreshing = true;
                if (await _database.DeleteFile(File.Guid) > 0)
                {
                    IsRefreshing = false;
                    Return();
                }
                else
                {
                    IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.FailedToDelete + " " + File.Name, AppRes.Ok);
                }
            }
        }


        public async void Return()
        {
            await MopupService.Instance.PopAsync();
        }

        public async void OpenCloseAdvancedDetails()
        {
            IsAdvancedDetails = !IsAdvancedDetails;
            SettFileSize();
        }

        private async void SettFileSize()
        {
            FileSize = await _database.GetFileContentSize(File.Guid);
        }



    }
}

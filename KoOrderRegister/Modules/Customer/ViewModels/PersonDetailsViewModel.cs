using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Customer.ViewModels
{
    public class PersonDetailsViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseModel _database;

        #region Commands
            public ICommand ReturnCommand => new Command(ClosePage);
            public ICommand SaveCommand => new Command(SavePerson);
            public ICommand DeleteCommand => new Command(DeletePerson);
        #endregion

        private CustomerModel _customer = new CustomerModel();
        public CustomerModel Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }
        private bool _isEdit = false;
        public bool IsEdit 
        {
            get => _isEdit;
            set
            {
                _isEdit = value;
                OnPropertyChanged(nameof(IsEdit));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public PersonDetailsViewModel(IDatabaseModel database)
        {
            _database = database;
        }

        public async void SavePerson()
        {
            int result = await _database.CreateCustomer(Customer);
            if(result == 1)
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.SuccessToSave + " " + Customer.Name, AppRes.Ok);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.Save, AppRes.FailedToSave + " " + Customer.Name, AppRes.Ok);
            }
        }

        public async void DeletePerson()
        {
            
            if (await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + Customer.Name, AppRes.Yes, AppRes.No))
            {
                if (Customer != null)
                {
                    int result = await _database.DeleteCustomer(Guid.Parse(Customer.Id));
                    if (result > 0)
                    {
                        ClosePage();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.FailedToDelete + " " + Customer.Name, AppRes.Ok);
                    }
                }
            }
                
        }
        public void ClosePage()
        {
            App.Current.MainPage.Navigation.PopAsync();
        }
    }
}

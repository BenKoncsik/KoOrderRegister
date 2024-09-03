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

        }

        public async Task DeletePerson()
        {
            if(Customer != null)
            {
                int result = await _database.DeleteCustomer(Guid.Parse(Customer.Id));
                if(result > 0)
                {
                    ClosePage();
                }
            }
                
        }
        public void ClosePage()
        {
            Customer = new CustomerModel();
            App.Current.MainPage.Navigation.PopAsync();
        }
    }
}

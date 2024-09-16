using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using Microsoft.Maui.Controls;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Customer.ViewModels
{
    public class CustomerListViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseModel _database;
        private PersonDetailsPage _personDetailsPage;
        private ShowCustomerPopUp _showCustomerPopUp;

        public event PropertyChangedEventHandler? PropertyChanged;
        public string SearchTXT { get; set; } = "";
        private CancellationTokenSource _searchCancellationTokenSource;
        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
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
        #region Commands
        public ICommand UpdateCommand => new Command(Update);
        public ICommand AddNewCustomerCommand => new Command(AddNewCustomer);
        public Command<CustomerModel> DeleteCustomerCommand => new Command<CustomerModel>(DeleteCustomer);
        public Command<CustomerModel> EditCustomerCommand => new Command<CustomerModel>(EditCustumer);
        public Command<CustomerModel> ToggleDetailsCommand => new Command<CustomerModel>(ToggleDetails);
        public Command<string> SearchCommand => new Command<string>(Search);
        #endregion
        public ObservableCollection<CustomerModel> Customers { get; set; } = new ObservableCollection<CustomerModel>();
        private bool _isDetailsVisible = false;
        public bool IsDetailsVisible
        {
            get => _isDetailsVisible;
            set
            {
                _isDetailsVisible = value;
            }
        }
        public CustomerListViewModel(IDatabaseModel database, 
            PersonDetailsPage personDetailsPage,
            ShowCustomerPopUp showCustomerPopUp)
        {
            _database = database;
            _personDetailsPage = personDetailsPage;
            _showCustomerPopUp = showCustomerPopUp;
        }

        public async void Update()
        {
            if (string.IsNullOrEmpty(SearchTXT))
            {
                IsLoading = true;
                if (Customers != null)
                {
                    Customers.Clear();
                }
                foreach (var customer in await _database.GetAllCustomers())
                {
                    Customers.Add(customer);
                }
                IsLoading = false;
            }
            else
            {
                Search(SearchTXT);
            }
            
        }

        public async void AddNewCustomer()
        {
            await App.Current.MainPage.Navigation.PushAsync(_personDetailsPage);
        }

        public async void EditCustumer(CustomerModel customer) 
        {
            _personDetailsPage.EditCustomer(customer);
            await App.Current.MainPage.Navigation.PushAsync(_personDetailsPage);
        }
        public async void DeleteCustomer(CustomerModel customer) 
        {
            bool result = await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + customer.Name, AppRes.Yes, AppRes.No);
            if (result)
            {
               int deleteResult = await _database.DeleteCustomer(customer.Guid);
                if(deleteResult == 1)
                {
                    Customers.Remove(customer);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.FailedToDelete + " " + customer.Name, AppRes.Ok);
                }
            }
        }


        public async void ToggleDetails(CustomerModel customer)
        {
            _showCustomerPopUp.EditCustomer(customer);
            await MopupService.Instance.PushAsync(_showCustomerPopUp);
        }


        public async void Search(string search)
        {
            _searchCancellationTokenSource?.Cancel();

            _searchCancellationTokenSource = new CancellationTokenSource();
            var token = _searchCancellationTokenSource.Token;

            try
            {

                await Task.Delay(300, token);

                if (!token.IsCancellationRequested)
                {
                    await PerformSearch(search);
                }
            }
            catch (TaskCanceledException)
            {

            }
        }

        private async Task PerformSearch(string search)
        {
            IsLoading = true;
            SearchTXT = search;
            Customers?.Clear();

            var searchResults = await _database.SearchCustomer(search);
            foreach (var cutomer in searchResults)
            {
                if (!Customers.Any(o => o.Id.Equals(cutomer.Id)))
                {
                    Customers?.Add(cutomer);
                }
            }

            IsLoading = false;
        }
    }
}

using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using Microsoft.Maui.Controls;
using Mopups.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
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

        private int updatePage = 1;
        private int searchPage = 1;
        private bool hasMoreUpdateItems = true;
        private bool hasMoreSearchItems = true;
        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
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
        public Command<CustomerModel> EditCustomerCommand => new Command<CustomerModel>(EditCustomer);
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
                if (_isDetailsVisible != value)
                {
                    _isDetailsVisible = value;
                    OnPropertyChanged(nameof(IsDetailsVisible));
                }
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
            hasMoreUpdateItems = true;
            updatePage = 1;
            _update();
        }

        private async void _update()
        {
            if (IsLoading || !hasMoreUpdateItems)
                return;

            IsLoading = updatePage == 1? true : false;
            hasMoreUpdateItems = true;

            var customers = await _database.GetAllCustomers(updatePage);

            if (customers.Count > 0)
            {
                foreach (var customer in customers)
                {
                    if (!Customers.Any(c => c.Id.Equals(customer.Id)))
                    {
                        Customers.Add(customer);
                    }
                }

                updatePage++;
            }
            else
            {
                hasMoreUpdateItems = false;
            }

            IsLoading = false;
            _update();
        }

        public async void AddNewCustomer()
        {
            await App.Current.MainPage.Navigation.PushAsync(_personDetailsPage);
        }

        public async void EditCustomer(CustomerModel customer)
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
                if (deleteResult == 1)
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

        public void Search(string search)
        {
            searchPage = 1;
            hasMoreSearchItems = true;
            Customers.Clear();

            _searchCancellationTokenSource?.Cancel();

            _searchCancellationTokenSource = new CancellationTokenSource();
            var token = _searchCancellationTokenSource.Token;

            try
            {
                Task.Delay(300, token).ContinueWith(async t =>
                {
                    if (!token.IsCancellationRequested)
                    {
                        await PerformSearch(search);
                    }
                }, token);
            }
            catch (TaskCanceledException)
            {

            }
        }

        private async Task PerformSearch(string search)
        {
            if (IsLoading || !hasMoreSearchItems)
            {
                return;
            }
                

            IsLoading = searchPage == 1? true : false;
            SearchTXT = search;

            var searchResults = await _database.SearchCustomer(search, searchPage);

            if (searchResults.Count > 0)
            {
                foreach (var customer in searchResults)
                {
                    if (!Customers.Any(o => o.Id.Equals(customer.Id)))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Customers?.Add(customer);
                        });
                        
                    }
                }

                searchPage++;
            }
            else
            {
                hasMoreSearchItems = false;
            }

            IsLoading = false;
            await PerformSearch(SearchTXT);
        }

        public void LoadMoreItems()
        {
            if (IsLoading)
                return;

            if (string.IsNullOrEmpty(SearchTXT))
            {
                Update();
            }
            else
            {
                _ = PerformSearch(SearchTXT);
            }
        }
    }
}

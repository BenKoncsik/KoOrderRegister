using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Services;
using KoOrderRegister.Utility;
using KoOrderRegister.ViewModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using KORCore.Utility;
using KORCore.Modules.Database.Factory;

namespace KoOrderRegister.Modules.Customer.ViewModels
{
    public class CustomerListViewModel : BaseViewModel
    {
        private readonly IDatabaseModel _database;

        private readonly PersonDetailsPage _personDetailsPage;
        #region Binding varrible


        #endregion


        public string SearchTXT { get; set; } = string.Empty;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


        #region Commands
        public ICommand UpdateCommand { get; }
        public ICommand AddNewCustomerCommand { get; }
        public Command<CustomerModel> DeleteCustomerCommand { get; }
        public Command<CustomerModel> EditCustomerCommand { get; }
        public Command<string> SearchCommand { get; }
        #endregion

        public ObservableCollection<CustomerModel> Customers { get; set; } = new ObservableCollection<CustomerModel>();
        
        public CustomerListViewModel(IDatabaseModelFactory database, PersonDetailsPage personDetailsPage, IAppUpdateService updateService, ILocalNotificationService notificationService) : base(updateService, notificationService)
        {
            _database = database.Get();
            _personDetailsPage = personDetailsPage;

        UpdateCommand = new Command(Update);
        AddNewCustomerCommand = new Command(AddNewCustomer);
        DeleteCustomerCommand = new Command<CustomerModel>(DeleteCustomer);
        EditCustomerCommand = new Command<CustomerModel>(EditCustomer);
        SearchCommand = new Command<string>(Search);
    }

        public async void Update()
        {
            using (new LowPriorityTaskManager())
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _cancellationTokenSource = new CancellationTokenSource();

                Customers.Clear();
                if (string.IsNullOrEmpty(SearchTXT))
                {
                    await _update();
                }
                else
                {
                    await _search(SearchTXT);
                }
            }
           
        }

        private async Task _update()
        {
            IsRefreshing = true;
            try
            {
                await foreach (CustomerModel customer in _database.GetAllCustomersAsStream(_cancellationTokenSource.Token))
                {
                    if (!Customers.Any(c => c.Id.Equals(customer.Id)))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Customers?.Add(customer);
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                IsRefreshing = false;
            }
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

        public async void Search(string search)
        {
            SearchTXT = search;
            Update();
        }


        private async Task _search(string search)
        {
            try
            {
                await foreach (CustomerModel customer in _database.SearchCustomerAsStream(search, _cancellationTokenSource.Token))
                {
                    if (!Customers.Any(o => o.Id.Equals(customer.Id)))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Customers?.Add(customer);
                        });
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }   
        }
    }
}

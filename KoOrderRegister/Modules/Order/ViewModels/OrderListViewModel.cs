using KoOrderRegister.Localization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Utility;
using KoOrderRegister.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Order.ViewModels
{
    public class OrderListViewModel : BaseViewModel
    {
        private readonly IDatabaseModel _database;
        private readonly OrderDetailsPage _orderDetailsPage;
        #region Binding varrible
       
        #endregion
        public string SearchTXT { get; set; } = string.Empty;
        private CancellationTokenSource _searchCancellationTokenSource;

        private int updatePage = 1;
        private int searchPage = 1;
        private bool hasMoreUpdateItems = true;
        private bool hasMoreSearchItems = true;

        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();

        #region Commands
        public ICommand AddNewOrderCommand { get; }
        public ICommand EditOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand UpdateOrderCommand { get; }
        public ICommand SearchCommand { get; }
        #endregion

        public OrderListViewModel(IDatabaseModel database, OrderDetailsPage orderDetailsPage)
        {
            _database = database;
            _orderDetailsPage = orderDetailsPage;

            AddNewOrderCommand = new Command(NewOrder);
            EditOrderCommand = new Command<OrderModel>(EditOrder);
            DeleteOrderCommand = new Command<OrderModel>(DeleteOrder);
            UpdateOrderCommand = new Command(UpdateOrders);
            SearchCommand = new Command<string>(Search);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
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

        public async void NewOrder()
        {
            await App.Current.MainPage.Navigation.PushAsync(_orderDetailsPage);
        }

        public async void EditOrder(OrderModel order)
        {
            _orderDetailsPage.EditOrder(order);
            await App.Current.MainPage.Navigation.PushAsync(_orderDetailsPage);
        }

        public async void UpdateOrders()
        {
            hasMoreUpdateItems = true;
            updatePage = 1;
            if (SearchTXT.Equals("") || SearchTXT.Equals(string.Empty))
            {
                await _updateOrders();
            }
            else
            {
                await PerformSearch(SearchTXT);
            }
            IsRefreshing = false;
        }
        private async Task _updateOrders()
        {
            if (!hasMoreUpdateItems)
            {
                return;
            }
               
            IsRefreshing = updatePage == 1 ? true : false;
            hasMoreUpdateItems = true;
            var orders = await _database.GetAllOrders(updatePage);

            if (orders.Count > 0)
            {
                foreach (var order in orders)
                {
                    if (!Orders.Any(o => o.Id.Equals(order.Id)))
                    {
                        Orders.Add(order);
                    }
                }

                updatePage++; 
            }
            else
            {
                hasMoreUpdateItems = false; 
            }

            IsRefreshing = false;
            if (hasMoreUpdateItems)
            {
                _updateOrders();
            }
        }

        public async void DeleteOrder(OrderModel order)
        {
            bool result = await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + order.OrderNumber, AppRes.Yes, AppRes.No);
            if (result)
            {
                if (await _database.DeleteOrder(order.Guid) > 0)
                {
                    Orders.Remove(order);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.FailedToDelete + " " + order.OrderNumber, AppRes.Ok);
                }
            }
        }

        public void Search(string search)
        {
            
            searchPage = 1;
            hasMoreSearchItems = true;
            Orders.Clear();

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
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task PerformSearch(string search)
        {
            if (!hasMoreSearchItems)
            {
                return;
            }

            IsRefreshing = searchPage == 1 ? true : false;
            SearchTXT = search;

            var searchResults = await _database.SearchOrders(search, searchPage);

            if (searchResults.Count > 0)
            {
                foreach (var order in searchResults)
                {
                    if (!Orders.Any(o => o.Id.Equals(order.Id)))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Orders?.Add(order);
                        });
                    }
                }

                searchPage++;   
            }
            else
            {
                hasMoreSearchItems = false; 
            }
            IsRefreshing = false;
            await PerformSearch(SearchTXT);
        }

        public void LoadMoreItems()
        {
            if (IsRefreshing)
                return;

            if (string.IsNullOrEmpty(SearchTXT))
            {
                UpdateOrders();
            }
            else
            {
                _ = PerformSearch(SearchTXT);
            }
        }
    }
}

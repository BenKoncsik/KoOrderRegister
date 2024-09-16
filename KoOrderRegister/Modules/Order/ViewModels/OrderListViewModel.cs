using KoOrderRegister.Localization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Order.ViewModels
{
    public class OrderListViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseModel _database;
        private readonly OrderDetailsPage _orderDetailsPage;
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
        public string SearchTXT { get; set; } = "";
        private CancellationTokenSource _searchCancellationTokenSource;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            
            if (string.IsNullOrEmpty(SearchTXT))
            {
                IsLoading = true;
                var orderRun = _database.GetAllOrders();
                if (Orders != null)
                {
                    Orders.Clear();
                }
                foreach (var order in await orderRun)
                {
                    Orders.Add(order);
                }
                IsLoading = false;
            }
            else
            {
                Search(SearchTXT);
            }
            
        }

        public async void DeleteOrder(OrderModel order)
        {
            if (await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete + " " + order.OrderNumber, AppRes.No, AppRes.Yes))
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
            Orders?.Clear();

            var searchResults = await _database.SearchOrders(search);
            foreach (var order in searchResults)
            {
                if (!Orders.Any(o => o.Id.Equals(order.Id)))
                {
                    Orders?.Add(order);
                }
            }

            IsLoading = false;
        }

    }
}

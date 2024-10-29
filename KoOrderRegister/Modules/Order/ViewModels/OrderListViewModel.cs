using KoOrderRegister.Localization;
using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Services;
using KoOrderRegister.Utility;
using KoOrderRegister.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using KORCore.Utility;
using KORCore.Modules.Database.Factory;

namespace KoOrderRegister.Modules.Order.ViewModels
{
    public class OrderListViewModel : BaseViewModel
    {
        private readonly IDatabaseModel _database;
        private readonly OrderDetailsPage _orderDetailsPage;
        #region Binding varrible
       
        #endregion
        public string SearchTXT { get; set; } = string.Empty;
        private CancellationTokenSource _cancellationTokenSource;

 
        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();

        #region Commands
        public ICommand AddNewOrderCommand { get; }
        public ICommand EditOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand UpdateOrderCommand { get; }
        public ICommand SearchCommand { get; }
        #endregion

        public OrderListViewModel(IDatabaseModelFactory database, OrderDetailsPage orderDetailsPage, IAppUpdateService updateService, ILocalNotificationService notificationService) : base(updateService, notificationService)
        {
            _database = database.Get();
            _orderDetailsPage = orderDetailsPage;

            AddNewOrderCommand = new Command(NewOrder);
            EditOrderCommand = new Command<OrderModel>(EditOrder);
            DeleteOrderCommand = new Command<OrderModel>(DeleteOrder);
            UpdateOrderCommand = new Command(UpdateOrders);
            SearchCommand = new Command<string>(Search);

#if DEBUG
            IDatabaseModel.OnDatabaseChange += TestReliTimeDatabase;
#endif
        }

#if DEBUG
        private void TestReliTimeDatabase(string name, object data)
        {
            Debug.WriteLine($"Name: {name} --> {data.GetType()}");
        }
#endif
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
            using (new LowPriorityTaskManager())
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                _cancellationTokenSource = new CancellationTokenSource();

                Orders.Clear();
                if (string.IsNullOrEmpty(SearchTXT))
                {
                    await _updateOrders();
                }
                else
                {
                    await _search(SearchTXT);
                }
            }
        }
        private async Task _updateOrders()
        {
            IsRefreshing = true;
            try
            {
                await foreach (OrderModel order in _database.GetAllOrdersAsStream(_cancellationTokenSource.Token))
                {
                    if (!Orders.Any(c => c.Id.Equals(order.Id)))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Orders?.Add(order);
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
            SearchTXT = search;
            UpdateOrders();
        }

        private async Task _search(string search)
        {
            IsRefreshing = true;
            try
            {
                await foreach (OrderModel order in _database.SearchOrdersAsStream(search, _cancellationTokenSource.Token))
                {
                    if (!Orders.Any(c => c.Id.Equals(order.Id)))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Orders?.Add(order);
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
    }
}

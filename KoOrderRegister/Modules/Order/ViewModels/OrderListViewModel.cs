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
        public string SearchTXT { get; set; } = "";
        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();
        #region Commands
        public ICommand AddNewOrderCommand => new Command(NewOrder);
        public Command<OrderModel> EditOrderCommand => new Command<OrderModel>(EditOrder);
        public Command<OrderModel> DeleteOrderCommand => new Command<OrderModel>(DeleteOrder);
        public ICommand UpdateOrderCommand => new Command(UpdateOrders);
        public Command<string> SearchCommand => new Command<string>(Search);
        #endregion
        public OrderListViewModel(IDatabaseModel database, OrderDetailsPage orderDetailsPage)
        {
            _database = database;
            _orderDetailsPage = orderDetailsPage;
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
                var orderRun = _database.GetAllOrders();
                if (Orders != null)
                {
                    Orders.Clear();
                }
                foreach (var order in await orderRun)
                {
                    Orders.Add(order);
                }
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
            SearchTXT = search;
            if (Orders != null)
            {
                Orders.Clear();
            }
            foreach (var order in await _database.SearchOrders(search))
            {
                Orders.Add(order);
            }
        }
    }
}

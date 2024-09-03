using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Order.ViewModels
{
    public class OrderListViewModel
    {
        private readonly IDatabaseModel _database;
        public ObservableCollection<OrderModel> Orders { get; set; }
        public OrderListViewModel(IDatabaseModel database)
        {
            _database = database;
        }

        public async void UpdateOrders()
        {
            Orders.Clear();
            foreach(var order in await _database.GetAllOrders())
            {
                Orders.Add(order);
            }
        }




    }
}

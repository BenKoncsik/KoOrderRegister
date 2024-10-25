using KORCore.Module.Database.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Socket.Client
{
   /* public class RealTimeDatabaseClient
    {
        private readonly HubConnection _connection;

        public event Action<string, object> DatabaseChanged;

        public RealTimeDatabaseClient(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            #region Customer
            _connection.On<CustomerModel>(DatabaseChangedType.CUSTOMER_CREATED, customer =>
            {
                Debug.WriteLine($"Customer created: {customer}");
                DatabaseChanged?.Invoke(DatabaseChangedType.CUSTOMER_CREATED, customer);
            });

            _connection.On<CustomerModel>(DatabaseChangedType.CUSTOMER_UPDATED, customer =>
            {
                Debug.WriteLine($"Customer updated: {customer}");
                DatabaseChanged?.Invoke(DatabaseChangedType.CUSTOMER_UPDATED, customer);
            });

            _connection.On<Guid>(DatabaseChangedType.CUSTOMER_DELETED, customerId =>
            {
                Debug.WriteLine($"Customer deleted: {customerId}");
                DatabaseChanged?.Invoke(DatabaseChangedType.CUSTOMER_DELETED, customerId);
            });

            _connection.On<int>(DatabaseChangedType.CUSTOMER_COUNT_CHANGED, count =>
            {
                Debug.WriteLine($"Customer count changed: {count}");
                DatabaseChanged?.Invoke(DatabaseChangedType.CUSTOMER_COUNT_CHANGED, count);
            });

            _connection.On<List<CustomerModel>>(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers =>
            {
                Debug.WriteLine($"Customers retrieved: {customers.Count}");
                DatabaseChanged?.Invoke(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers);
            });

            _connection.On<Guid>(DatabaseChangedType.NOTIFY_CUSTOMER_DELETED, customersId =>
            {
                Debug.WriteLine($"Customers deleted: {customersId}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_CUSTOMER_DELETED, customersId);
            });
            _connection.On<CustomerModel>(DatabaseChangedType.NOTIFY_CUSTOMER_CREATED, customer =>
            {
                Console.WriteLine($"Notify: Customer created: {customer}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_CUSTOMER_CREATED, customer);
            });

            _connection.On<CustomerModel>(DatabaseChangedType.NOTIFY_CUSTOMER_UPDATED, customer =>
            {
                Console.WriteLine($"Notify: Customer updated: {customer}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_CUSTOMER_UPDATED, customer);
            });

            _connection.On<Guid>(DatabaseChangedType.NOTIFY_CUSTOMER_DELETED, customerId =>
            {
                Console.WriteLine($"Notify: Customer deleted: {customerId}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_CUSTOMER_DELETED, customerId);
            });
            #endregion
            #region Order
            _connection.On<OrderModel>(DatabaseChangedType.ORDER_CREATED, order =>
            {
                Debug.WriteLine($"Order created: {order}");
                DatabaseChanged?.Invoke(DatabaseChangedType.ORDER_CREATED, order);
            });

            _connection.On<OrderModel>(DatabaseChangedType.ORDER_UPDATED, order =>
            {
                Debug.WriteLine($"Order updated: {order}");
                DatabaseChanged?.Invoke(DatabaseChangedType.ORDER_UPDATED, order);
            });

            _connection.On<Guid>(DatabaseChangedType.ORDER_DELETED, orderId =>
            {
                Debug.WriteLine($"Order deleted: {orderId}");
                DatabaseChanged?.Invoke(DatabaseChangedType.ORDER_DELETED, orderId);
            });

            _connection.On<int>(DatabaseChangedType.ORDER_COUNT_CHANGED, count =>
            {
                Debug.WriteLine($"Order count changed: {count}");
                DatabaseChanged?.Invoke(DatabaseChangedType.ORDER_COUNT_CHANGED, count);
            });

            _connection.On<List<OrderModel>>(DatabaseChangedType.ORDERS_RETRIEVED, orders =>
            {
                Debug.WriteLine($"Orders retrieved: {orders.Count}");
                DatabaseChanged?.Invoke(DatabaseChangedType.ORDERS_RETRIEVED, orders);
            });
            _connection.On<OrderModel>(DatabaseChangedType.NOTIFY_ORDER_CREATED, order =>
            {
                Console.WriteLine($"Notify: Order created: {order}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_ORDER_CREATED, order);
            });

            _connection.On<OrderModel>(DatabaseChangedType.NOTIFY_ORDER_UPDATED, order =>
            {
                Console.WriteLine($"Notify: Order updated: {order}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_ORDER_UPDATED, order);
            });

            _connection.On<Guid>(DatabaseChangedType.NOTIFY_ORDER_DELETED, orderId =>
            {
                Console.WriteLine($"Notify: Order deleted: {orderId}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_ORDER_DELETED, orderId);
            });
            #endregion
            #region File
            _connection.On<FileModel>(DatabaseChangedType.FILE_CREATED, file =>
            {
                Debug.WriteLine($"File created: {file}");
                DatabaseChanged?.Invoke(DatabaseChangedType.FILE_CREATED, file);
            });

            _connection.On<FileModel>(DatabaseChangedType.FILE_UPDATED, file =>
            {
                Debug.WriteLine($"File updated: {file}");
                DatabaseChanged?.Invoke(DatabaseChangedType.FILE_UPDATED, file);
            });

            _connection.On<Guid>(DatabaseChangedType.FILE_DELETED, fileId =>
            {
                Debug.WriteLine($"File deleted: {fileId}");
                DatabaseChanged?.Invoke(DatabaseChangedType.FILE_DELETED, fileId);
            });

            _connection.On<int>(DatabaseChangedType.FILE_COUNT_CHANGED, count =>
            {
                Debug.WriteLine($"File count changed: {count}");
                DatabaseChanged?.Invoke(DatabaseChangedType.FILE_COUNT_CHANGED, count);
            });

            _connection.On<List<FileModel>>(DatabaseChangedType.FILES_RETRIEVED, files =>
            {
                Debug.WriteLine($"Files retrieved: {files.Count}");
                DatabaseChanged?.Invoke(DatabaseChangedType.FILES_RETRIEVED, files);
            });
            _connection.On<FileModel>(DatabaseChangedType.NOTIFY_FILE_CREATED, file =>
            {
                Console.WriteLine($"Notify: File created: {file}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_FILE_CREATED, file);
            });

            _connection.On<FileModel>(DatabaseChangedType.NOTIFY_FILE_UPDATED, file =>
            {
                Console.WriteLine($"Notify: File updated: {file}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_FILE_UPDATED, file);
            });

            _connection.On<Guid>(DatabaseChangedType.NOTIFY_FILE_DELETED, fileId =>
            {
                Console.WriteLine($"Notify: File deleted: {fileId}");
                DatabaseChanged?.Invoke(DatabaseChangedType.NOTIFY_FILE_DELETED, fileId);
            });
            #endregion
        }
        public async Task NotifyCustomerCreated(CustomerModel model)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_CUSTOMER_CREATED, model);
        }

        public async Task NotifyCustomerUpdated(CustomerModel model)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_CUSTOMER_UPDATED, model);
        }

        public async Task NotifyCustomerDeleted(Guid customerId)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_CUSTOMER_DELETED, customerId);
        }

        public async Task NotifyOrderCreated(OrderModel model)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_ORDER_CREATED, model);
        }

        public async Task NotifyOrderUpdated(OrderModel model)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_ORDER_UPDATED, model);
        }

        public async Task NotifyOrderDeleted(Guid orderId)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_ORDER_DELETED, orderId);
        }

        public async Task NotifyFileCreated(FileModel model)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_FILE_CREATED, model);
        }

        public async Task NotifyFileUpdated(FileModel model)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_FILE_UPDATED, model);
        }

        public async Task NotifyFileDeleted(Guid fileId)
        {
            await _connection.SendAsync(DatabaseChangedType.NOTIFY_FILE_DELETED, fileId);
        }

        public async Task StartAsync()
        {
            await _connection.StartAsync();
            Debug.WriteLine("Connected to the real-time database hub.");
        }

        public async Task StopAsync()
        {
            await _connection.StopAsync();
            Debug.WriteLine("Disconnected from the real-time database hub.");
        }
    }*/
}

using KoOrderRegister.Modules.Database.Socket.Client;
using KORCore.Module.Database.Utility;
using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Socket.Server;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Services
{
 /*   public class RealTimeDatabaseModel : Hub, IDatabaseModel
    {
        private readonly IDatabaseModel _innerDatabaseModel;
        private readonly IHubContext<RealTimeDatabaseHub> _hubContext;
        private readonly RealTimeDatabaseClient _realTimeDatabaseClient;

        public RealTimeDatabaseModel(IDatabaseModel innerDatabaseModel, IHubContext<RealTimeDatabaseHub> hubContext, RealTimeDatabaseClient realTimeDatabaseClient)
        {
            _innerDatabaseModel = innerDatabaseModel;
            _hubContext = hubContext;
            _realTimeDatabaseClient = realTimeDatabaseClient;
            _realTimeDatabaseClient.DatabaseChanged += RealTimeDatabaseClient_DatabaseChanged;
        }

        private async void RealTimeDatabaseClient_DatabaseChanged(string name, object vale)
        {
            switch (name)
            {
                case DatabaseChangedType.NOTIFY_CUSTOMER_CREATED:
                    await CreateCustomer((CustomerModel)vale);
                    break;
                case DatabaseChangedType.NOTIFY_CUSTOMER_UPDATED:
                    await UpdateCustomer((CustomerModel)vale);
                    break;
                case DatabaseChangedType.NOTIFY_CUSTOMER_DELETED:
                    await DeleteCustomer((Guid)vale);
                    break;
                case DatabaseChangedType.NOTIFY_ORDER_CREATED:
                    await CreateOrder((OrderModel)vale);
                    break;
                case DatabaseChangedType.NOTIFY_ORDER_UPDATED:
                    await UpdateOrder((OrderModel)vale);
                    break;
                case DatabaseChangedType.NOTIFY_ORDER_DELETED:
                    await DeleteOrder((Guid)vale);
                    break;
                case DatabaseChangedType.NOTIFY_FILE_CREATED:
                    await CreateFile((FileModel)vale);
                    break;
                case DatabaseChangedType.NOTIFY_FILE_UPDATED:
                    await UpdateFile((FileModel)vale);
                    break;
                case DatabaseChangedType.NOTIFY_FILE_DELETED:
                    await DeleteFile((Guid)vale);
                    break;

            }
        }

        #region CustomerModel CRUD Operations
        public async Task<int> CreateCustomer(CustomerModel customer)
        {
            int result = await _innerDatabaseModel.CreateCustomer(customer);
            if (result > 0)
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_CREATED, customer);
            }
            return result;
        }

        public async Task<CustomerModel> GetCustomerById(Guid id)
        {
            CustomerModel customer = await _innerDatabaseModel.GetCustomerById(id);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_RETRIEVED, customer);
            return customer;
        }

        public async Task<List<CustomerModel>> GetAllCustomers(int page = int.MinValue)
        {
            List<CustomerModel> customers = await _innerDatabaseModel.GetAllCustomers(page);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers);
            return customers;
        }

        public async IAsyncEnumerable<CustomerModel> GetAllCustomersAsStream(CancellationToken cancellationToken)
        {
            await foreach (var customer in _innerDatabaseModel.GetAllCustomersAsStream(cancellationToken))
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_STREAM_RETRIEVED, customer);
                yield return customer;
            }
        }

        public async Task<int> UpdateCustomer(CustomerModel customer)
        {
            int result = await _innerDatabaseModel.UpdateCustomer(customer);
            if (result > 0)
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_UPDATED, customer);
            }
            return result;
        }

        public async Task<int> DeleteCustomer(Guid id)
        {
            int result = await _innerDatabaseModel.DeleteCustomer(id);
            if (result > 0)
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_DELETED, id);
            }
            return result;
        }

        public async Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue)
        {
            List<CustomerModel> customers = await _innerDatabaseModel.SearchCustomer(search, page);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers);
            return customers;
        }

        public async IAsyncEnumerable<CustomerModel> SearchCustomerAsStream(string search, CancellationToken cancellationToken)
        {
            await foreach (var customer in _innerDatabaseModel.SearchCustomerAsStream(search, cancellationToken))
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_STREAM_RETRIEVED, customer);
                yield return customer;
            }
        }

        public async Task<int> CountCustomers()
        {
            int result = await _innerDatabaseModel.CountCustomers();
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_COUNT_CHANGED, result);
            return result;
        }
        #endregion

        #region OrderModel CRUD Operations
        public async Task<int> CreateOrder(OrderModel order)
        {
            int result = await _innerDatabaseModel.CreateOrder(order);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDER_CREATED, order);
            return result;
        }

        public async Task<OrderModel> GetOrderById(Guid id)
        {
            OrderModel order = await _innerDatabaseModel.GetOrderById(id);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDER_RETRIEVED, order);
            return order;
        }

        public async Task<List<OrderModel>> GetAllOrders(int page = int.MinValue)
        {
            List<OrderModel> orders = await _innerDatabaseModel.GetAllOrders(page);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDERS_RETRIEVED, orders);
            return orders;
        }

        public async IAsyncEnumerable<OrderModel> GetAllOrdersAsStream(CancellationToken cancellationToken)
        {
            await foreach (var order in _innerDatabaseModel.GetAllOrdersAsStream(cancellationToken))
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDER_STREAM_RETRIEVED, order);
                yield return order;
            }
        }

        public async Task<int> UpdateOrder(OrderModel order)
        {
            int result = await _innerDatabaseModel.UpdateOrder(order);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDER_UPDATED, order);
            return result;
        }

        public async Task<int> DeleteOrder(Guid id)
        {
            int result = await _innerDatabaseModel.DeleteOrder(id);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDER_DELETED, id);
            return result;
        }

        public async Task<List<OrderModel>> SearchOrders(string search, int page = int.MinValue)
        {
            List<OrderModel> orders = await _innerDatabaseModel.SearchOrders(search, page);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDERS_RETRIEVED, orders);
            return orders;
        }

        public async IAsyncEnumerable<OrderModel> SearchOrdersAsStream(string search, CancellationToken cancellationToken)
        {
            await foreach (var order in _innerDatabaseModel.SearchOrdersAsStream(search, cancellationToken))
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDER_STREAM_RETRIEVED, order);
                yield return order;
            }
        }

        public async Task<int> CountOrders()
        {
            int result = await _innerDatabaseModel.CountOrders();
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.ORDER_COUNT_CHANGED, result);
            return result;
        }
        #endregion

        #region FileModel CRUD Operations
        public async Task<int> CreateFile(FileModel file)
        {
            int result = await _innerDatabaseModel.CreateFile(file);
            if (result > 0)
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_CREATED, file);
            }
            return result;
        }

        public async Task<FileModel> GetFileById(Guid id)
        {
            FileModel file = await _innerDatabaseModel.GetFileById(id);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_RETRIEVED, file);
            return file;
        }

        public async Task<List<FileModel>> GetAllFilesByOrderId(Guid id)
        {
            List<FileModel> files = await _innerDatabaseModel.GetAllFilesByOrderId(id);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILES_RETRIEVED, files);
            return files;
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesByOrderIdAsStream(Guid id, CancellationToken cancellationToken)
        {
            await foreach (var file in _innerDatabaseModel.GetAllFilesByOrderIdAsStream(id, cancellationToken))
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_STREAM_RETRIEVED, file);
                yield return file;
            }
        }

        public async Task<List<FileModel>> GetFilesByOrderIdWithOutContent(Guid id)
        {
            List<FileModel> files = await _innerDatabaseModel.GetFilesByOrderIdWithOutContent(id);
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILES_RETRIEVED, files);
            return files;
        }

        public async IAsyncEnumerable<FileModel> GetFilesByOrderIdWithOutContentAsStream(Guid id, CancellationToken cancellationToken)
        {
            await foreach (var file in _innerDatabaseModel.GetFilesByOrderIdWithOutContentAsStream(id, cancellationToken))
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_STREAM_RETRIEVED, file);
                yield return file;
            }
        }

        public async Task<List<FileModel>> GetAllFiles()
        {
            List<FileModel> files = await _innerDatabaseModel.GetAllFiles();
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILES_RETRIEVED, files);
            return files;
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesAsStream(CancellationToken cancellationToken)
        {
            await foreach (var file in _innerDatabaseModel.GetAllFilesAsStream(cancellationToken))
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_STREAM_RETRIEVED, file);
                yield return file;
            }
        }

        public async Task<int> UpdateFile(FileModel file)
        {
            int result = await _innerDatabaseModel.UpdateFile(file);
            if (result > 0)
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_UPDATED, file);
            }
            return result;
        }

        public async Task<int> DeleteFile(Guid id)
        {
            int result = await _innerDatabaseModel.DeleteFile(id);
            if (result > 0)
            {
                await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_DELETED, id);
            }
            return result;
        }

        public async Task<string> GetFileContentSize(Guid id)
        {
            string size = await _innerDatabaseModel.GetFileContentSize(id);
            return size;
        }

        public async Task<int> CountFiles()
        {
            int result = await _innerDatabaseModel.CountFiles();
            await _hubContext.Clients.All.SendAsync(DatabaseChangedType.FILE_COUNT_CHANGED, result);
            return result;
        }
        #endregion

        #region Database Export/Import Operations
        public async Task ExportDatabaseToJson(string filePath, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            await _innerDatabaseModel.ExportDatabaseToJson(filePath, cancellationToken, progressCallback);
        }

        public async Task ImportDatabaseFromJson(Stream jsonStream, Action<float> progressCallback = null)
        {
            await _innerDatabaseModel.ImportDatabaseFromJson(jsonStream, progressCallback);
        }
        #endregion
    }*/

}

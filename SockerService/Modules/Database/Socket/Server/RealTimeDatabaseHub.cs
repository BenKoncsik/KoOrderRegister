using KORCore.Modules.Database.Socket.Utility;
using KORCore.Module.Database.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockerService.Modules.Database.Socket.Server
{
    public class RealTimeDatabaseHub : Hub
    {
        public async Task NotifyCustomerCreated(CustomerModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_CREATED, model);
        }

        public async Task NotifyCustomerUpdated(CustomerModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_UPDATED, model);
        }

        public async Task NotifyCustomerDeleted(Guid customerId)
        {
            await Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_DELETED, customerId);
        }

        public async Task NotifyCustomerCountChanged(int count)
        {
            await Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_COUNT_CHANGED, count);
        }

        public async Task NotifyCustomerRetrieved(CustomerModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_RETRIEVED, model);
        }

        public async Task NotifyCustomersRetrieved(List<CustomerModel> customers)
        {
            await Clients.All.SendAsync(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers);
        }

        public async Task NotifyCustomerStreamRetrieved(IAsyncEnumerable<CustomerModel> customers)
        {
            await Clients.All.SendAsync(DatabaseChangedType.CUSTOMER_STREAM_RETRIEVED, customers);
        }

        public async Task NotifyOrderCreated(OrderModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.ORDER_CREATED, model);
        }

        public async Task NotifyOrderUpdated(OrderModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.ORDER_UPDATED, model);
        }

        public async Task NotifyOrderDeleted(Guid orderId)
        {
            await Clients.All.SendAsync(DatabaseChangedType.ORDER_DELETED, orderId);
        }

        public async Task NotifyOrderCountChanged(int count)
        {
            await Clients.All.SendAsync(DatabaseChangedType.ORDER_COUNT_CHANGED, count);
        }

        public async Task NotifyOrderRetrieved(OrderModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.ORDER_RETRIEVED, model);
        }

        public async Task NotifyOrdersRetrieved(List<OrderModel> orders)
        {
            await Clients.All.SendAsync(DatabaseChangedType.ORDERS_RETRIEVED, orders);
        }

        public async Task NotifyOrderStreamRetrieved(IAsyncEnumerable<OrderModel> orders)
        {
            await Clients.All.SendAsync(DatabaseChangedType.ORDER_STREAM_RETRIEVED, orders);
        }

        public async Task NotifyFileCreated(FileModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.FILE_CREATED, model);
        }

        public async Task NotifyFileUpdated(FileModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.FILE_UPDATED, model);
        }

        public async Task NotifyFileDeleted(Guid fileId)
        {
            await Clients.All.SendAsync(DatabaseChangedType.FILE_DELETED, fileId);
        }

        public async Task NotifyFileCountChanged(int count)
        {
            await Clients.All.SendAsync(DatabaseChangedType.FILE_COUNT_CHANGED, count);
        }

        public async Task NotifyFileRetrieved(FileModel model)
        {
            await Clients.All.SendAsync(DatabaseChangedType.FILE_RETRIEVED, model);
        }

        public async Task NotifyFilesRetrieved(List<FileModel> files)
        {
            await Clients.All.SendAsync(DatabaseChangedType.FILES_RETRIEVED, files);
        }

        public async Task NotifyFileStreamRetrieved(IAsyncEnumerable<FileModel> files)
        {
            await Clients.All.SendAsync(DatabaseChangedType.FILE_STREAM_RETRIEVED, files);
        }
    }
}

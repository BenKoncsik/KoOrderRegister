using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Socket.Utility
{
    public static class DatabaseChangedType
    {
        public const string CUSTOMER_CREATED = "CustomerCreated";
        public const string CUSTOMER_UPDATED = "CustomerUpdated";
        public const string CUSTOMER_DELETED = "CustomerDeleted";
        public const string CUSTOMER_COUNT_CHANGED = "CustomerCountChanged";
        public const string CUSTOMER_RETRIEVED = "CustomerRetrieved";
        public const string CUSTOMERS_RETRIEVED = "CustomersRetrieved";
        public const string CUSTOMER_STREAM_RETRIEVED = "CustomerStreamRetrieved";
        public const string NOTIFY_CUSTOMER_CREATED = "NotifyCustomerCreated";
        public const string NOTIFY_CUSTOMER_UPDATED = "NotifyCustomerUpdated";
        public const string NOTIFY_CUSTOMER_DELETED = "NotifyCustomerDeleted";

        public const string ORDER_CREATED = "OrderCreated";
        public const string ORDER_UPDATED = "OrderUpdated";
        public const string ORDER_DELETED = "OrderDeleted";
        public const string ORDER_COUNT_CHANGED = "OrderCountChanged";
        public const string ORDER_RETRIEVED = "OrderRetrieved";
        public const string ORDERS_RETRIEVED = "OrdersRetrieved";
        public const string ORDER_STREAM_RETRIEVED = "OrderStreamRetrieved";
        public const string NOTIFY_ORDER_CREATED = "NotifyOrderCreated";
        public const string NOTIFY_ORDER_UPDATED = "NotifyOrderUpdated";
        public const string NOTIFY_ORDER_DELETED = "NotifyOrderDeleted";

        public const string FILE_CREATED = "FileCreated";
        public const string FILE_UPDATED = "FileUpdated";
        public const string FILE_DELETED = "FileDeleted";
        public const string FILE_COUNT_CHANGED = "FileCountChanged";
        public const string FILE_RETRIEVED = "FileRetrieved";
        public const string FILES_RETRIEVED = "FilesRetrieved";
        public const string FILE_STREAM_RETRIEVED = "FileStreamRetrieved";
        public const string NOTIFY_FILE_CREATED = "NotifyFileCreated";
        public const string NOTIFY_FILE_UPDATED = "NotifyFileUpdated";
        public const string NOTIFY_FILE_DELETED = "NotifyFiledelete";
    }
}

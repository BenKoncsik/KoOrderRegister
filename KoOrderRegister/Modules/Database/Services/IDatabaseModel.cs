using KORCore.Module.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Services
{
    public interface IDatabaseModel
    {
        #region CustomerModel CRUD Operations
        Task<int> CreateCustomer(CustomerModel customer);
        Task<CustomerModel> GetCustomerById(Guid id);
        Task<List<CustomerModel>> GetAllCustomers(int page = int.MinValue);
        IAsyncEnumerable<CustomerModel> GetAllCustomersAsStream(CancellationToken cancellationToken);
        Task<int> UpdateCustomer(CustomerModel customer);
        Task<int> DeleteCustomer(Guid id);
        Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue);
        IAsyncEnumerable<CustomerModel> SearchCustomerAsStream(string search, CancellationToken cancellationToken);
        Task<int> CountCustomers();
        #endregion

        #region OrderModel CRUD Operations
        Task<int> CreateOrder(OrderModel order);
        Task<OrderModel> GetOrderById(Guid id);
        Task<List<OrderModel>> GetAllOrders(int page = int.MinValue);
        IAsyncEnumerable<OrderModel> GetAllOrdersAsStream(CancellationToken cancellationToken);
        Task<int> UpdateOrder(OrderModel order);
        Task<int> DeleteOrder(Guid id);
        Task<List<OrderModel>> SearchOrders(string search, int page = int.MinValue);
        IAsyncEnumerable<OrderModel> SearchOrdersAsStream(string search, CancellationToken cancellationToken);
        Task<int> CountOrders();
        #endregion
        #region FileModel CRUD Operations
        Task<int> CreateFile(FileModel file);
        Task<FileModel> GetFileById(Guid id);
        Task<List<FileModel>> GetAllFilesByOrderId(Guid id);
        IAsyncEnumerable<FileModel> GetAllFilesByOrderIdAsStream(Guid id, CancellationToken cancellationToken);
        Task<List<FileModel>> GetFilesByOrderIdWithOutContent(Guid id);
        IAsyncEnumerable<FileModel> GetFilesByOrderIdWithOutContentAsStream(Guid id, CancellationToken cancellationToken);
        Task<List<FileModel>> GetAllFiles();
        IAsyncEnumerable<FileModel> GetAllFilesAsStream(CancellationToken cancellationToken);
        Task<int> UpdateFile(FileModel file);
        Task<int> DeleteFile(Guid id);
        Task<string> GetFileContentSize(Guid id);
        Task<int> CountFiles();
        #endregion
        #region Database Export/Import Operations
        Task ExportDatabaseToJson(string filePath, CancellationToken cancellationToken, Action<float> progressCallback = null);
        Task ImportDatabaseFromJson(Stream jsonStream, Action<float> progressCallback = null);
        #endregion
    }
}

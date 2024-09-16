using KoOrderRegister.Modules.Database.Models;
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
        Task<int> UpdateCustomer(CustomerModel customer);
        Task<int> DeleteCustomer(Guid id);
        Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue);
        #endregion

        #region OrderModel CRUD Operations
        Task<int> CreateOrder(OrderModel order);
        Task<OrderModel> GetOrderById(Guid id);
        Task<List<OrderModel>> GetAllOrders(int page = int.MinValue);
        Task<int> UpdateOrder(OrderModel order);
        Task<int> DeleteOrder(Guid id);
        Task<List<OrderModel>> SearchOrders(string search, int page = int.MinValue);
        #region
        #endregion FileModel CRUD Operations
        Task<int> CreateFile(FileModel file);
        Task<FileModel> GetFileById(Guid id);
        Task<List<FileModel>> GetAllFilesByOrderId(Guid id);
        Task<List<FileModel>> GetAllFiles();
        Task<int> UpdateFile(FileModel file);
        Task<int> DeleteFile(Guid id);
        #endregion
        #region Database Export/Import Operations
        Task<string> ExportDatabaseToJson();
        Task ImportDatabaseFromJson(string jsonData);
        #endregion
    }
}

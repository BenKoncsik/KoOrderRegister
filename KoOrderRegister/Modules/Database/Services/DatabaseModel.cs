using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Utility;
using Newtonsoft.Json;
using SQLite;
namespace KoOrderRegister.Modules.Database.Services
{
    public class DatabaseModel : IDatabaseModel
    {
        private SQLiteAsyncConnection Database;
        private SQLiteConnectionString options;

        

        public DatabaseModel()
        {
            Task.Run(async () => await Init()).Wait();
        }
        private async Task Init()
        {
            if (Database is not null)
            {
                return;
            }
            try
            {
                Database = new SQLiteAsyncConnection(Constants.basePath, Constants.Flags);
                await Database.CreateTableAsync<CustomerModel>();
                await Database.CreateTableAsync<OrderModel>();
                await Database.CreateTableAsync<FileModel>();
                options = new SQLiteConnectionString(Constants.basePath, false);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            

        }

        #region CustomerModel CRUD Implementation
        public async Task<int> CreateCustomer(CustomerModel customer)
        {
            CustomerModel? result = await GetCustomerById(customer.Guid);
            if(result == null)
            {
                return await Database.InsertAsync(customer);
            }
            else
            {
                return await UpdateCustomer(customer);
            }
            
        }
        public async Task<CustomerModel> GetCustomerById(Guid id)
        {
            string stringId = id.ToString();
            var customer = await Database.FindAsync<CustomerModel>(stringId);
            if (customer != null)
            {
                var orders = await Database.Table<OrderModel>().Where(o => o.CustomerId.Equals(stringId)).ToListAsync();
                foreach (var order in orders)
                {
                    order.Files = await Database.Table<FileModel>().Where(f => f.OrderId.Equals(order.Id)).ToListAsync();
                }
                customer.Orders = orders;
            }
            return customer;
        }


        public async Task<List<CustomerModel>> GetAllCustomers()
        {
            var customers = await Database.Table<CustomerModel>().ToListAsync();
            foreach (var customer in customers)
            {
                var orders = await Database.Table<OrderModel>().Where(o => o.CustomerId == customer.Id).ToListAsync();
                foreach (var order in orders)
                {
                    order.Files = await Database.Table<FileModel>().Where(f => f.OrderId == order.Id).ToListAsync();
                }
                customer.Orders = orders;
            }
            return customers;
        }


        public async Task<int> UpdateCustomer(CustomerModel customer)
        {
            return await Database.UpdateAsync(customer);
        }

        public async Task<int> DeleteCustomer(Guid id)
        {
            string stringId = id.ToString();  // Convert Guid to string
            var customer = await GetCustomerById(id);
            if (customer != null)
            {
                foreach (var order in customer.Orders)
                {
                    await DeleteOrder(Guid.Parse(order.Id));
                }
                return await Database.DeleteAsync(customer);
            }
            return 0;
        }

        public async Task<List<CustomerModel>> SearchCustomer(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await GetAllCustomers();
            }

            string likeQuery = $"%{search.Trim().ToLowerInvariant().Replace(" ", "%")}%";

            var query = $@"SELECT * FROM Customers 
                           WHERE LOWER(Name) LIKE ? OR 
                                 LOWER(Address) LIKE ? OR
                                 LOWER(Phone) LIKE ? OR
                                 LOWER(Email) LIKE ? OR
                                 LOWER(Note) LIKE ? OR
                                 LOWER(NationalHealthInsurance) LIKE ?";

            return await Database.QueryAsync<CustomerModel>(query, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery);
        }
        #endregion
        #region OrderModel CRUD Implementation
        public async Task<int> CreateOrder(OrderModel order)
        {
            OrderModel? result = await GetOrderById(order.Guid);
            if(result == null)
            {
                return await Database.InsertAsync(order);
            }
            else
            {
                return await UpdateOrder(order);
            }
        }

        public async Task<OrderModel> GetOrderById(Guid id)
        {
            string stringId = id.ToString();  // Convert Guid to string
            var order = await Database.FindAsync<OrderModel>(stringId);
            if (order != null)
            {
                order.Files = await Database.Table<FileModel>().Where(f => f.OrderId.Equals(stringId)).ToListAsync();
            }
            return order;
        }

        public async Task<List<OrderModel>> GetAllOrders()
        {
            var orders = await Database.Table<OrderModel>().ToListAsync();
            var tasks = orders.Select(order =>
                 ThreadManager.Run(async () =>
                 {
                     var fileCount = await Database.Table<FileModel>().Where(f => f.OrderId.Equals(order.Id)).CountAsync();
                     if (fileCount > 0)
                     {
                         order.Files = await Database.Table<FileModel>().Where(f => f.OrderId.Equals(order.Id)).ToListAsync();
                     }
                     if (!string.IsNullOrEmpty(order.CustomerId))
                     {
                         order.Customer = await GetCustomerById(Guid.Parse(order.CustomerId));
                     }
                 }));

            await Task.WhenAll(tasks);
            return orders;
        }

        public async Task<int> UpdateOrder(OrderModel order)
        {
            return await Database.UpdateAsync(order);
        }

        public async Task<int> DeleteOrder(Guid id)
        {
            string stringId = id.ToString();  // Convert Guid to string
            var order = await GetOrderById(id);
            if (order != null)
            {
                foreach (var file in order.Files)
                {
                    await DeleteFile(Guid.Parse(file.Id));
                }
                return await Database.DeleteAsync(order);
            }
            return 0;
        }

        public async Task<List<OrderModel>> SearchOrders(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await GetAllOrders();
            }
            string likeQuery = $"%{search.Trim().ToLowerInvariant().Replace(" ", "%")}%";
            var query = $@"SELECT o.* FROM Orders o
                           JOIN Customers c ON o.CustomerId = c.Id
                           WHERE LOWER(o.OrderNumber) LIKE ? OR 
                           LOWER(o.Note) LIKE ? OR
                           LOWER(c.Name) LIKE ? OR
                           LOWER(c.Address) LIKE ? OR
                           LOWER(c.Phone) LIKE ? OR
                           LOWER(c.Email) LIKE ? OR
                           LOWER(c.NationalHealthInsurance) LIKE ?";


            List<OrderModel> orders = await Database.QueryAsync<OrderModel>(query, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery);

            List<Task> tasks = new List<Task>();
            foreach (var order in orders)
            {
                tasks.Add(ThreadManager.Run(async () =>
                {
                    if (!string.IsNullOrEmpty(order.CustomerId))
                    {
                        order.Customer = await GetCustomerById(Guid.Parse(order.CustomerId));
                    }
                }));
            }
            await Task.WhenAll(tasks);
            return orders;
        }
        #endregion
        #region FileModel CRUD Implementation
        public async Task<int> CreateFile(FileModel file)
        {
            FileModel? result = await GetFileById(file.Guid);
            if(result == null)
            {
                return await Database.InsertAsync(file);
            }
            else
            {
                return await UpdateFile(file);
            }
            
        }

        public async Task<FileModel> GetFileById(Guid id)
        {
            string stringId = id.ToString();  // Convert Guid to string
            return await Database.FindAsync<FileModel>(stringId);
        }

        public async Task<List<FileModel>> GetAllFilesByOrderId(Guid id)
        {
            string stringId = id.ToString();  // Convert Guid to string
            var fileCount = await Database.Table<FileModel>().Where(f => f.OrderId.Equals(stringId)).CountAsync();
            if (fileCount > 0)
            {
                return await Database.Table<FileModel>().Where(f => f.OrderId.Equals(stringId)).ToListAsync();
            }
            return new List<FileModel>();
        }

        public async Task<List<FileModel>> GetAllFiles()
        {
            return await Database.Table<FileModel>().ToListAsync();
        }

        public async Task<int> UpdateFile(FileModel file)
        {
            return await Database.UpdateAsync(file);
        }

        public async Task<int> DeleteFile(Guid id)
        {
            string stringId = id.ToString();  // Convert Guid to string
            var file = await GetFileById(id);
            if (file != null)
            {
                return await Database.DeleteAsync(file);
            }
            return 0;
        }
        #endregion

        #region export/import
        public async Task<string> ExportDatabaseToJson()
        {
            var customers = await GetAllCustomers();
            var orders = await GetAllOrders();
            var files = await GetAllFiles();
            var databaseExport = new
            {
                Customers = customers,
                Orders = orders,
                Files = files
            };
            return JsonConvert.SerializeObject(databaseExport);
        }

        public async Task ImportDatabaseFromJson(string jsonData)
        {
            var databaseImport = JsonConvert.DeserializeObject<DatabaseImportModel>(jsonData);
            await Database.DropTableAsync<CustomerModel>();
            await Database.DropTableAsync<OrderModel>();
            await Database.DropTableAsync<FileModel>();

            await Init(); 
            foreach (var customer in databaseImport.Customers)
            {
                await CreateCustomer(customer);
            }
            foreach (var order in databaseImport.Orders)
            {
                await CreateOrder(order);
            }
            foreach (var file in databaseImport.Files)
            {
                await CreateFile(file);
            }
        }

        public class DatabaseImportModel
        {
            public List<CustomerModel> Customers { get; set; }
            public List<OrderModel> Orders { get; set; }
            public List<FileModel> Files { get; set; }
        }

        #endregion
    }


    public static class Constants
    {
#if DEBUG
        public const string DatabaseFilename = "order_debug.db";
#else
        public const string DatabaseFilename = "order.db";
#endif

        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        public static string basePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DatabaseFilename);
            //Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}

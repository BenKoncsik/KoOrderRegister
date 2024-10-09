using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Utility;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using SQLite;
using System.Collections.Concurrent;
using System.Text;

namespace KoOrderRegister.Modules.Database.Services
{
    public class DatabaseModel : IDatabaseModel
    {
        private SQLiteAsyncConnection Database;
        private SQLiteConnectionString options;

        private static string CUSTOMER_TABLE = "Customers";
        private static string ORDER_TABLE = "Orders";
        private static string FILES_TABLE = "Files";

        private static int PAGE_SIZE = 5;


        public DatabaseModel()
        {
            Task.Run(async () => await Init()).Wait();
        }
        private async Task Init(bool force = false)
        {
            if (Database is not null && !force)
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
                var orders = await Database.Table<OrderModel>()
                    .Where(o => o.CustomerId.Equals(stringId))
                    .ToListAsync();
                foreach (var order in orders)
                {
                    order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
                }
                customer.Orders = orders;
            }
            return customer;
        }


        public async Task<List<CustomerModel>> GetAllCustomers(int page = int.MinValue)
        {
            if(page == int.MinValue)
            {
                var customers = await Database.Table<CustomerModel>().ToListAsync();
                foreach (var customer in customers)
                {
                    var orders = await Database.Table<OrderModel>()
                        .Where(o => o.CustomerId == customer.Id)
                        .ToListAsync();
                    foreach (var order in orders)
                    {
                        order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
                    }
                    customer.Orders = orders;
                }
                return customers;
            }
            else
            {
                var customers = await Database.Table<CustomerModel>()
                    .ThenBy(c => c.Name)
                    .Skip((page - 1) * PAGE_SIZE)
                    .Take(PAGE_SIZE)
                    .ToListAsync();
                foreach (var customer in customers)
                {
                    var orders = await Database.Table<OrderModel>()
                        .Where(o => o.CustomerId == customer.Id)
                        .ToListAsync();
                    foreach (var order in orders)
                    {
                        order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
                    }
                    customer.Orders = orders;
                }
                return customers;
            }
        }

        public async IAsyncEnumerable<CustomerModel> GetAllCustomersAsStream(CancellationToken cancellationToken)
        {
                var customers = await Database.Table<CustomerModel>().ToListAsync();
                foreach (var customer in customers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var orders = await Database.Table<OrderModel>()
                                .Where(o => o.CustomerId == customer.Id)
                                .ToListAsync();

                            foreach (var order in orders)
                            {
                                await foreach (FileModel file in GetFilesByOrderIdWithOutContentAsStream(order.Guid, cancellationToken))
                                {
                                    if (file != null)
                                    {
                                        order.Files.Add(file);
                                    }
                                }
                            }
                            customer.Orders = orders;
                            yield return customer;
                }
        }


        public async Task<int> UpdateCustomer(CustomerModel customer)
        {
            return await Database.UpdateAsync(customer);
        }

        public async Task<int> DeleteCustomer(Guid id)
        {
            string stringId = id.ToString(); 
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

        public async Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await GetAllCustomers();
            }
            string likeQuery = $"%{search.Trim().ToLowerInvariant().Replace(" ", "%")}%";
            List<CustomerModel> customers = new List<CustomerModel>();

            if (page == int.MinValue)
            {
                string query = $@"SELECT * FROM {CUSTOMER_TABLE} 
                       WHERE LOWER(Name) LIKE ? OR 
                       LOWER(Address) LIKE ? OR
                       LOWER(Phone) LIKE ? OR
                       LOWER(Email) LIKE ? OR
                       LOWER(Note) LIKE ? OR
                       LOWER(NationalHealthInsurance) LIKE ?";
                customers = await Database.QueryAsync<CustomerModel>(query, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery);
            }
            else
            {
                string query = $@"
                      SELECT * FROM {CUSTOMER_TABLE}
                      WHERE LOWER(Name) LIKE ? OR 
                      LOWER(Address) LIKE ? OR
                      LOWER(Phone) LIKE ? OR
                      LOWER(Email) LIKE ? OR
                      LOWER(Note) LIKE ? OR
                      LOWER(NationalHealthInsurance) LIKE ?
                      LIMIT ?
                      OFFSET ?";
               customers = await Database.QueryAsync<CustomerModel>(query, likeQuery, likeQuery, likeQuery, 
                    likeQuery, likeQuery, likeQuery, PAGE_SIZE,  (PAGE_SIZE * (page - 1)));
            }
           return customers
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .ToList();

        }

        public async IAsyncEnumerable<CustomerModel> SearchCustomerAsStream(string search, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(search))
            {
                await foreach(var customer in GetAllCustomersAsStream(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return customer;                    
                }
            }
            string likeQuery = $"%{search.Trim().ToLowerInvariant().Replace(" ", "%")}%";
            string query = $@"SELECT * FROM {CUSTOMER_TABLE} 
                       WHERE LOWER(Name) LIKE ? OR 
                       LOWER(Address) LIKE ? OR
                       LOWER(Phone) LIKE ? OR
                       LOWER(Email) LIKE ? OR
                       LOWER(Note) LIKE ? OR
                       LOWER(NationalHealthInsurance) LIKE ?";
            List<CustomerModel>  customers = await Database.QueryAsync<CustomerModel>(query, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery);

            foreach (var customer in customers.GroupBy(c => c.Id).Select(g => g.First()).ToList())
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return customer;
            }
        }

        public async Task<int> CountCustomers()
        {
            var count = await Database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {CUSTOMER_TABLE}");
            return count;
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
            string stringId = id.ToString();  
            var order = await Database.FindAsync<OrderModel>(stringId);
            if (order != null)
            {
                order.Files = order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
            }
            return order;
        }

        public async Task<List<OrderModel>> GetAllOrders(int page = int.MinValue)
        {
            List<OrderModel> orders = new List<OrderModel>();
            if (page == int.MinValue)
            {
                orders = await Database.Table<OrderModel>()
                    .OrderBy(o => o.StartDate)
                    .ToListAsync();  
            }
            else
            {
                orders = await Database.Table<OrderModel>()
                    .ThenBy(o => o.StartDate)
                    .Skip((page - 1) * PAGE_SIZE)
                    .Take(PAGE_SIZE).ToListAsync();
            }
            var tasks = orders.Select(order =>
                    ThreadManager.Run(async () =>
                    {
                        var fileCount = await Database.Table<FileModel>()
                        .Where(f => f.OrderId.Equals(order.Id))
                        .CountAsync();
                        if (fileCount > 0)
                        {
                            order.Files = order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
                        }
                        if (!string.IsNullOrEmpty(order.CustomerId))
                        {
                            order.Customer = await GetCustomerById(Guid.Parse(order.CustomerId));
                        }
                    }));

            await Task.WhenAll(tasks);
            return orders;
        }

        

        public async IAsyncEnumerable<OrderModel> GetAllOrdersAsStream(CancellationToken cancellationToken)
        {
            List<OrderModel> orders = await Database.Table<OrderModel>()
                    .OrderBy(o => o.StartDate)
                    .ToListAsync();
            foreach (var order in orders)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fileCount = await Database.Table<FileModel>()
                    .Where(f => f.OrderId.Equals(order.Id))
                    .CountAsync();
                if (fileCount > 0)
                {
                    order.Files = order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
                }
                if (!string.IsNullOrEmpty(order.CustomerId))
                {
                    order.Customer = await GetCustomerById(Guid.Parse(order.CustomerId));
                }
                yield return order;
            }
        }

        public async Task<int> UpdateOrder(OrderModel order)
        {
            return await Database.UpdateAsync(order);
        }

        public async Task<int> DeleteOrder(Guid id)
        {
            string stringId = id.ToString();  
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

        public async Task<List<OrderModel>> SearchOrders(string search, int page = int.MinValue)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await GetAllOrders();
            }
            string likeQuery = $"%{search.Trim().ToLowerInvariant().Replace(" ", "%")}%";
            List<OrderModel> orders = new List<OrderModel>();


            if (page == int.MinValue)
            {
                var query = $@"SELECT o.* FROM {ORDER_TABLE} o
                           JOIN {CUSTOMER_TABLE} c ON o.CustomerId = c.Id
                           WHERE LOWER(o.OrderNumber) LIKE ? OR 
                           LOWER(o.Note) LIKE ? OR
                           LOWER(c.Name) LIKE ? OR
                           LOWER(c.Address) LIKE ? OR
                           LOWER(c.Phone) LIKE ? OR
                           LOWER(c.Email) LIKE ? OR
                           LOWER(c.NationalHealthInsurance) LIKE ?";


                orders = await Database.QueryAsync<OrderModel>(query, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery);
            }
            else
            {
                var query = $@"SELECT o.* FROM {ORDER_TABLE} o
                           JOIN {CUSTOMER_TABLE} c ON o.CustomerId = c.Id
                           WHERE LOWER(o.OrderNumber) LIKE ? OR 
                           LOWER(o.Note) LIKE ? OR
                           LOWER(c.Name) LIKE ? OR
                           LOWER(c.Address) LIKE ? OR
                           LOWER(c.Phone) LIKE ? OR
                           LOWER(c.Email) LIKE ? OR
                           LOWER(c.NationalHealthInsurance) LIKE ?
                           LIMIT ?
                           OFFSET ?";


                orders = await Database.QueryAsync<OrderModel>(query, likeQuery, 
                    likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, 
                    PAGE_SIZE, (PAGE_SIZE * (page - 1)));
            }
            
            orders = orders
                .GroupBy(c => c.Id)
                .Select(g => g.First())
                .ToList();

            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();

            void RunBackgroundTask(OrderModel order)
            {
                tasks.Add(ThreadManager.Run(async () =>
                {
                    if (!string.IsNullOrEmpty(order.CustomerId))
                    {
                        order.Customer = await GetCustomerById(Guid.Parse(order.CustomerId));
                        order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
                    }
                }));
            }

            orders.AsParallel().ForAll(order => RunBackgroundTask(order));
            await Task.WhenAll(tasks);
            return orders;
        }

        public async IAsyncEnumerable<OrderModel> SearchOrdersAsStream(string search, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(search))
            {
                await foreach(var order in GetAllOrdersAsStream(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return order;
                } 
            }
            string likeQuery = $"%{search.Trim().ToLowerInvariant().Replace(" ", "%")}%";          
            var query = $@"SELECT o.* FROM {ORDER_TABLE} o
                        JOIN {CUSTOMER_TABLE} c ON o.CustomerId = c.Id
                        WHERE LOWER(o.OrderNumber) LIKE ? OR 
                        LOWER(o.Note) LIKE ? OR
                        LOWER(c.Name) LIKE ? OR
                        LOWER(c.Address) LIKE ? OR
                        LOWER(c.Phone) LIKE ? OR
                        LOWER(c.Email) LIKE ? OR
                        LOWER(c.NationalHealthInsurance) LIKE ?";

            List<OrderModel>  orders = await Database.QueryAsync<OrderModel>(query, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery, likeQuery);

            foreach(var order in orders.GroupBy(c => c.Id).Select(g => g.First()).ToList())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrEmpty(order.CustomerId))
                {
                    order.Customer = await GetCustomerById(Guid.Parse(order.CustomerId));
                    order.Files = await GetFilesByOrderIdWithOutContent(order.Guid);
                }
                yield return order;
            }
        }

        public async Task<int> CountOrders()
        {
            var count = await Database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {ORDER_TABLE}");
            return count;
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
            string stringId = id.ToString();  
            return await Database.FindAsync<FileModel>(stringId);
        }

        public async Task<List<FileModel>> GetAllFilesByOrderId(Guid id)
        {
            string stringId = id.ToString();  
            var fileCount = await Database.Table<FileModel>()
                .Where(f => f.OrderId.Equals(stringId))
                .CountAsync();
            if (fileCount > 0)
            {
                return await Database.Table<FileModel>()
                    .Where(f => f.OrderId.Equals(stringId))
                    .ToListAsync();
            }
            return new List<FileModel>();
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesByOrderIdAsStream(Guid id, CancellationToken cancellationToken)
        {
            string stringId = id.ToString();
            var fileCount = await Database.Table<FileModel>()
                .Where(f => f.OrderId.Equals(stringId))
                .CountAsync();
            if (fileCount > 0)
            {
                foreach(var file in await Database.Table<FileModel>().Where(f => f.OrderId.Equals(stringId)).ToListAsync())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return file;  
                }
            }
            yield return null;
        }

        public async Task<List<FileModel>> GetAllFiles()
        {
            List<FileModel> files = await Database.Table<FileModel>().ToListAsync();
            files.AsParallel().Where(f => f.IsDatabaseContent = true);
            return files;
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesAsStream(CancellationToken cancellationToken)
        {
            List<FileModel> files = await Database.Table<FileModel>().ToListAsync();
            foreach(var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                file.IsDatabaseContent = true;
                file.Order = await GetOrderById(Guid.Parse(file.OrderId));
                yield return file;
            }
        }

        public async Task<int> UpdateFile(FileModel file)
        {
            return await Database.UpdateAsync(file);
        }

        public async Task<int> DeleteFile(Guid id)
        {
            
            string stringId = id.ToString();  
            var file = await GetFileById(id);
            if (file != null)
            {
                return await Database.DeleteAsync(file);   
            }
            return 0;
        }

        public async Task<List<FileModel>> GetFilesByOrderIdWithOutContent(Guid id)
        {
            var query = $"SELECT id, orderId, name, contentType, note, hashCode FROM {FILES_TABLE} WHERE orderId = ?";
            List<FileModel> fileModels = await Database.QueryAsync<FileModel>(query, id.ToString());
            foreach(var file in fileModels)
            {
                file.IsDatabaseContent = true;
            }
            return fileModels;
        }

        public async IAsyncEnumerable<FileModel> GetFilesByOrderIdWithOutContentAsStream(Guid id, CancellationToken cancellationToken)
        {
            var query = $"SELECT id, orderId, name, contentType, note, hashCode FROM {FILES_TABLE} WHERE orderId = ?";
            foreach (var file in await Database.QueryAsync<FileModel>(query, id.ToString()))
            {
                cancellationToken.ThrowIfCancellationRequested();
                file.IsDatabaseContent = true;
                yield return file;
            }
        }

        public async Task<string> GetFileContentSize(Guid id)
        {
            var query = $"SELECT SUM(length(content)) FROM {FILES_TABLE} WHERE id = ?";
            long length = await Database.ExecuteScalarAsync<long>(query, id.ToString());
            return length.ToStringSize();
        }
        public async Task<int> CountFiles()
        {
            var count = await Database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {FILES_TABLE}");
            return count;
        }
        #endregion

        #region export/import
        public async Task ExportDatabaseToJson(string filePath, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            int totalItems = await CountCustomers() + await CountOrders() + await CountFiles();
            int processedItems = 0;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            using (StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false)))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                JsonSerializer serializer = new JsonSerializer();

                jsonWriter.WriteStartObject();
                // Serialize Customers
                jsonWriter.WritePropertyName("Customers");
                jsonWriter.WriteStartArray();
                await foreach (var customer in GetAllCustomersAsStream(cancellationToken))
                {
                    serializer.Serialize(jsonWriter, customer);
                    processedItems++;
                    progressCallback?.Invoke(100 * processedItems / totalItems);
                }
                jsonWriter.WriteEndArray();

                // Serialize Orders
                jsonWriter.WritePropertyName("Orders");
                jsonWriter.WriteStartArray();
                await foreach (var order in GetAllOrdersAsStream(cancellationToken))
                {
                    serializer.Serialize(jsonWriter, order);
                    processedItems++;
                    progressCallback?.Invoke(100 * processedItems / totalItems);
                }
                jsonWriter.WriteEndArray();

                // Serialize Files
                jsonWriter.WritePropertyName("Files");
                jsonWriter.WriteStartArray();
                await foreach (var file in GetAllFilesAsStream(cancellationToken))
                {
                    serializer.Serialize(jsonWriter, file);
                    processedItems++;
                    progressCallback?.Invoke(100 * processedItems / totalItems);
                }
                jsonWriter.WriteEndArray();

                jsonWriter.WriteEndObject();
                await jsonWriter.FlushAsync();
            }
        }

        public async Task ImportDatabaseFromJson(Stream jsonStream, Action<float> progressCallback = null)
        {
            ProgressState state = new ProgressState(jsonStream.Length);
            using (StreamReader streamReader = new StreamReader(jsonStream))
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();

                if (!jsonReader.Read() || jsonReader.TokenType != JsonToken.StartObject)
                {
                    throw new JsonSerializationException("Expected start of JSON object.");
                }
                await Database.DropTableAsync<CustomerModel>();
                await Database.DropTableAsync<OrderModel>();
                await Database.DropTableAsync<FileModel>();
                await Init(force: true);

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = jsonReader.Value.ToString();
                        if (propertyName == CUSTOMER_TABLE)
                        {
                            state.StreamPosition = 10;
                            await ProcessItems<CustomerModel>(jsonReader, serializer, CreateCustomer, state, progressCallback);
                            state.StreamPosition = 33;
                        }
                        else if (propertyName == ORDER_TABLE)
                        {
                            state.StreamPosition = 40;
                            await ProcessItems<OrderModel>(jsonReader, serializer, CreateOrder, state, progressCallback);
                            state.StreamPosition = 66;
                        }
                        else if (propertyName == FILES_TABLE)
                        {
                            state.StreamPosition = 70;
                            await ProcessItems<FileModel>(jsonReader, serializer, CreateFile, state, progressCallback);
                            state.StreamPosition = 100;
                        }
                    }
                }
            }

        }

        private async Task ProcessItems<T>(JsonTextReader jsonReader, JsonSerializer serializer, Func<T, Task> createFunc, ProgressState state, Action<float> progressCallback)
        {
            if (jsonReader.Read() && jsonReader.TokenType == JsonToken.StartArray)
            {
                
                while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                {
                   T item = serializer.Deserialize<T>(jsonReader);
                   await createFunc(item);
                   state.UpdateProgress(progressCallback);
                }
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


    internal static class Constants
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
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),  DatabaseFilename);
    }

    internal class ProgressState
    {
        //TODO: Implement ProgressState
        public long StreamPosition { get; set; }
        public long FileSize { get; }
        internal ProgressState(long fileSize)
        {
            FileSize = fileSize;
        }
        public void UpdateProgress(Action<float> progressCallback)
        {
            if (progressCallback != null)
            {
                /*    float progressPercentage = (float)StreamPosition / FileSize * 100;
                    progressCallback?.Invoke((float)Math.Round(progressPercentage, 2));*/
                progressCallback?.Invoke((float)StreamPosition);
            }

        }
    }

}

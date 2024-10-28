using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Services
{
    public class RemoteDatabaseModel: IDatabaseModel 
    {
        private static readonly string ApiBaseUrl = "http://localhost:5000/api";
        private readonly HttpClient _httpClient;
        public RemoteDatabaseModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public static event Action<string, object> OnDatabaseChange;

        public static void InVokeOnDatabaseChange(string name, object data)
        {
            OnDatabaseChange?.Invoke(name, data);
        }

        #region CustomerModel CRUD Implementation
        public async Task<int> CreateCustomer(CustomerModel customer)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/customer", customer);
                response.EnsureSuccessStatusCode();
                InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_CREATED, customer);
                return 1;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException($"Error creating customer: {e.Message}", e);
            }
        }

        public async Task<CustomerModel> GetCustomerById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer/{id}");
                response.EnsureSuccessStatusCode();
                var customer = await response.Content.ReadFromJsonAsync<CustomerModel>();
                InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_RETRIEVED, customer);
                return customer;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException($"Error retrieving customer with ID {id}: {e.Message}", e);
            }
        }

        public async Task<List<CustomerModel>> GetAllCustomers(int page = int.MinValue)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer");
                response.EnsureSuccessStatusCode();
                var customers = await response.Content.ReadFromJsonAsync<List<CustomerModel>>();
                InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers);
                return customers;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException("Error retrieving all customers: " + e.Message, e);
            }
        }

        public async IAsyncEnumerable<CustomerModel> GetAllCustomersAsStream(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/customer/stream");
            HttpResponseMessage response = null;
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new JsonSerializer();

                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    while (await jsonTextReader.ReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                    {
                        if (jsonTextReader.TokenType == JsonToken.StartObject)
                        {
                            var customer = serializer.Deserialize<CustomerModel>(jsonTextReader);
                            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_STREAM_RETRIEVED, customer);
                            yield return customer;
                        }
                    }
                }
        }

        public async Task<int> UpdateCustomer(CustomerModel customer)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/customer", customer);
            response.EnsureSuccessStatusCode();
            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_UPDATED, customer);
            return await Task.FromResult(1);
        }

        public async Task<int> DeleteCustomer(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/customer/{id}");
                response.EnsureSuccessStatusCode();
                InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_DELETED, id);
                return await Task.FromResult(1);
            } 
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException("Error retrieving all customers: " + e.Message, e);
            }
    
        }

        public async Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer/search?search={search}&page={page}");
                response.EnsureSuccessStatusCode();
                List<CustomerModel> customers = await response.Content.ReadFromJsonAsync<List<CustomerModel>>();
                InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers);
                return await Task.FromResult(customers);
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException("Error retrieving all customers: " + e.Message, e);
            }
        }

        public async IAsyncEnumerable<CustomerModel> SearchCustomerAsStream(string search, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/customer/search/stream?search={search}");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                while (jsonTextReader.Read())
                {
                    if (jsonTextReader.TokenType == JsonToken.StartObject)
                    {
                        var customer = serializer.Deserialize<CustomerModel>(jsonTextReader);
                        InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_STREAM_RETRIEVED, customer);
                        yield return customer;
                    }
                }
            }
        }

        public async Task<int> CountCustomers()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer/count");
                response.EnsureSuccessStatusCode();
                int count = await response.Content.ReadFromJsonAsync<int>();
                InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_COUNT_CHANGED, count);
                return await Task.FromResult(count);
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException("Error retrieving all customers: " + e.Message, e);
            }

        }
        #endregion

        #region OrderModel CRUD Implementation
        public async Task<int> CreateOrder(OrderModel order)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/order", order);
                response.EnsureSuccessStatusCode();
                InVokeOnDatabaseChange(DatabaseChangedType.ORDER_CREATED, order);
                return 1;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException($"Error creating order: {e.Message}", e);
            }
        }

        public async Task<OrderModel> GetOrderById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order/{id}");
                response.EnsureSuccessStatusCode();
                var order = await response.Content.ReadFromJsonAsync<OrderModel>();
                InVokeOnDatabaseChange(DatabaseChangedType.ORDER_RETRIEVED, order);
                return order;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException($"Error retrieving order with ID {id}: {e.Message}", e);
            }
        }

        public async Task<List<OrderModel>> GetAllOrders(int page = int.MinValue)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order");
                response.EnsureSuccessStatusCode();
                var orders = await response.Content.ReadFromJsonAsync<List<OrderModel>>();
                InVokeOnDatabaseChange(DatabaseChangedType.ORDERS_RETRIEVED, orders);
                return orders;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException("Error retrieving all orders: " + e.Message, e);
            }
        }

        public async IAsyncEnumerable<OrderModel> GetAllOrdersAsStream(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/order/stream");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                while (await jsonTextReader.ReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                {
                    if (jsonTextReader.TokenType == JsonToken.StartObject)
                    {
                        var order = serializer.Deserialize<OrderModel>(jsonTextReader);
                        InVokeOnDatabaseChange(DatabaseChangedType.ORDER_STREAM_RETRIEVED, order);
                        yield return order;
                    }
                }
            }
        }

        public async Task<int> UpdateOrder(OrderModel order)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/order", order);
                response.EnsureSuccessStatusCode();
                InVokeOnDatabaseChange(DatabaseChangedType.ORDER_UPDATED, order);
                return 1;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException($"Error updating order: {e.Message}", e);
            }
        }

        public async Task<int> DeleteOrder(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/order/{id}");
            response.EnsureSuccessStatusCode();
            InVokeOnDatabaseChange(DatabaseChangedType.ORDER_DELETED, id);
            return await Task.FromResult(1);
        }

        public async Task<List<OrderModel>> SearchOrders(string search, int page = int.MinValue)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order/search?search={search}&page={page}");
                response.EnsureSuccessStatusCode();
                List<OrderModel> orders = await response.Content.ReadFromJsonAsync<List<OrderModel>>();
                InVokeOnDatabaseChange(DatabaseChangedType.ORDERS_RETRIEVED, orders);
                return await Task.FromResult(orders);
            }
            catch(HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException($"Error updating order: {e.Message}", e);
            }
            
        }

        public async IAsyncEnumerable<OrderModel> SearchOrdersAsStream(string search, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/order/search/stream?search={search}");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                while (await jsonTextReader.ReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                {
                    if (jsonTextReader.TokenType == JsonToken.StartObject)
                    {
                        var order = serializer.Deserialize<OrderModel>(jsonTextReader);
                        InVokeOnDatabaseChange(DatabaseChangedType.ORDER_STREAM_RETRIEVED, order);
                        yield return order;
                    }
                }
            }
        }

        public async Task<int> CountOrders()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order/count");
                response.EnsureSuccessStatusCode();
                int count = await response.Content.ReadFromJsonAsync<int>();
                InVokeOnDatabaseChange(DatabaseChangedType.ORDER_COUNT_CHANGED, count);
                return count;
            }
            catch (HttpRequestException e)
            {
                // Log the error or handle it appropriately
                throw new ApplicationException("Error counting orders: " + e.Message, e);
            }
        }
        #endregion

        #region FileModel CRUD Implementation
        public async Task<int> CreateFile(FileModel file)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/file", file);
                response.EnsureSuccessStatusCode();
                InVokeOnDatabaseChange(DatabaseChangedType.FILE_CREATED, file);
                return 1;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error creating file: {e.Message}", e);
            }
        }

        public async Task<FileModel> GetFileById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/{id}");
                response.EnsureSuccessStatusCode();
                var file = await response.Content.ReadFromJsonAsync<FileModel>();
                InVokeOnDatabaseChange(DatabaseChangedType.FILE_RETRIEVED, file);
                return file;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error retrieving file with ID {id}: {e.Message}", e);
            }
        }

        public async Task<List<FileModel>> GetAllFilesByOrderId(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/order/{id}");
                response.EnsureSuccessStatusCode();
                var files = await response.Content.ReadFromJsonAsync<List<FileModel>>();
                InVokeOnDatabaseChange(DatabaseChangedType.FILES_RETRIEVED, files);
                return files;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error retrieving files for order ID {id}: {e.Message}", e);
            }
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesByOrderIdAsStream(Guid id, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/file/order/{id}/stream");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                while (await jsonTextReader.ReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                {
                    if (jsonTextReader.TokenType == JsonToken.StartObject)
                    {
                        var file = serializer.Deserialize<FileModel>(jsonTextReader);
                        InVokeOnDatabaseChange(DatabaseChangedType.FILE_STREAM_RETRIEVED, file);
                        yield return file;
                    }
                }
            }
        }


        public async Task<List<FileModel>> GetFilesByOrderIdWithOutContent(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/order/{id}/withoutcontent");
                response.EnsureSuccessStatusCode();
                var files = await response.Content.ReadFromJsonAsync<List<FileModel>>();
                InVokeOnDatabaseChange(DatabaseChangedType.FILES_RETRIEVED, files);
                return files;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error retrieving files without content for order ID {id}: {e.Message}", e);
            }
        }

        public async IAsyncEnumerable<FileModel> GetFilesByOrderIdWithOutContentAsStream(Guid id, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/file/order/{id}/withoutcontent/stream");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                while (await jsonTextReader.ReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                {
                    if (jsonTextReader.TokenType == JsonToken.StartObject)
                    {
                        var file = serializer.Deserialize<FileModel>(jsonTextReader);
                        InVokeOnDatabaseChange(DatabaseChangedType.FILE_STREAM_RETRIEVED, file);
                        yield return file;
                    }
                }
            }
        }


        public async Task<List<FileModel>> GetAllFiles()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file");
                response.EnsureSuccessStatusCode();
                var files = await response.Content.ReadFromJsonAsync<List<FileModel>>();
                InVokeOnDatabaseChange(DatabaseChangedType.FILES_RETRIEVED, files);
                return files;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException("Error retrieving all files: " + e.Message, e);
            }
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesAsStream(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/file/stream");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                while (await jsonTextReader.ReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
                {
                    if (jsonTextReader.TokenType == JsonToken.StartObject)
                    {
                        var file = serializer.Deserialize<FileModel>(jsonTextReader);
                        InVokeOnDatabaseChange(DatabaseChangedType.FILE_STREAM_RETRIEVED, file);
                        yield return file;
                    }
                }
            }
        }

        public async Task<int> UpdateFile(FileModel file)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiBaseUrl}/file", file);
                response.EnsureSuccessStatusCode();
                InVokeOnDatabaseChange(DatabaseChangedType.FILE_UPDATED, file);
                return 1;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error updating file: {e.Message}", e);
            }
        }

        public async Task<int> DeleteFile(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/file/{id}");
                response.EnsureSuccessStatusCode();
                InVokeOnDatabaseChange(DatabaseChangedType.FILE_DELETED, id);
                return 1;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error deleting file with ID {id}: {e.Message}", e);
            }
        }

        public async Task<string> GetFileContentSize(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/content/size/{id}");
                response.EnsureSuccessStatusCode();
                string size = await response.Content.ReadAsStringAsync();
                InVokeOnDatabaseChange(DatabaseChangedType.FILE_SIZE, size);
                return size;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException($"Error getting content size of file with ID {id}: {e.Message}", e);
            }
        }

        public async Task<int> CountFiles()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/count");
                response.EnsureSuccessStatusCode();
                int count = await response.Content.ReadFromJsonAsync<int>();
                InVokeOnDatabaseChange(DatabaseChangedType.FILE_COUNT_CHANGED, count);
                return count;
            }
            catch (HttpRequestException e)
            {
                throw new ApplicationException("Error counting files: " + e.Message, e);
            }
        }
        #endregion

        #region Database Export/Import Implementation
        public async Task ExportDatabaseToJson(string filePath, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            // Logic to export the database to JSON
            progressCallback?.Invoke(100);
            InVokeOnDatabaseChange(DatabaseChangedType.NOTIFY_CUSTOMER_CREATED, filePath);
            await Task.CompletedTask;
        }

        public async Task ImportDatabaseFromJson(Stream jsonStream, Action<float> progressCallback = null)
        {
            // Logic to import the database from JSON
            progressCallback?.Invoke(100);
            InVokeOnDatabaseChange(DatabaseChangedType.NOTIFY_CUSTOMER_UPDATED, jsonStream);
            await Task.CompletedTask;
        }
        #endregion
    }
}

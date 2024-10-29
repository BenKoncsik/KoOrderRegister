using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Utility;
using KORCore.Modules.Remote.Utility;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Json;

namespace KORCore.Modules.Database.Services
{
    public class RemoteDatabaseModel: IDatabaseModel 
    {
        private static string ApiBaseUrl = "http://localhost:5000/api";
        private readonly HttpClient _httpClient;
        public RemoteDatabaseModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("kor_connection_client");
        }
        
        public static void SetUrl(string url)
        {
            ApiBaseUrl = url + "/api";
        }

        public static event Action<string, object> OnDatabaseChange;

        public static void InVokeOnDatabaseChange(string name, object data)
        {
            OnDatabaseChange?.Invoke(name, data);
        }

        #region CustomerModel CRUD Implementation
        public async Task<int> CreateCustomer(CustomerModel customer)
        {
            var response = await _httpClient.PostAsJsonAsyncNewtonsoft($"{ApiBaseUrl}/customer", customer);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to create customer. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_CREATED, customer);
            return 1;
        }

        public async Task<CustomerModel> GetCustomerById(Guid id)
        {
           
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get customer. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new CustomerModel();
            }
            var customer = await response.Content.ReadFromJsonAsyncNewtonsoft<CustomerModel>();
            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_RETRIEVED, customer);
            return customer;
            
        }

        public async Task<List<CustomerModel>> GetAllCustomers(int page = int.MinValue)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Failed to get all customer. StatusCode: {response.StatusCode}, Content: {errorContent}");
                    return new List<CustomerModel>();
                }
                var customers = await response.Content.ReadFromJsonAsyncNewtonsoft<List<CustomerModel>>();
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
            var response = await _httpClient.PutAsJsonAsyncNewtonsoft($"{ApiBaseUrl}/customer", customer);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to update customer. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_UPDATED, customer);
            return 1;
        }

        public async Task<int> DeleteCustomer(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/customer/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get customer. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_DELETED, id);
            return 1;
        }

        public async Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue)
        {

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer/search?search={search}&page={page}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get customer. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new List<CustomerModel>();
            }
            List<CustomerModel> customers = await response.Content.ReadFromJsonAsyncNewtonsoft<List<CustomerModel>>();
            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMERS_RETRIEVED, customers);
            return customers;
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
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/customer/count");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get customer. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            int count = await response.Content.ReadFromJsonAsyncNewtonsoft<int>();
            InVokeOnDatabaseChange(DatabaseChangedType.CUSTOMER_COUNT_CHANGED, count);
            return count;

        }
        #endregion

        #region OrderModel CRUD Implementation
        public async Task<int> CreateOrder(OrderModel order)
        {
            var response = await _httpClient.PostAsJsonAsyncNewtonsoft($"{ApiBaseUrl}/order", order);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get order. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.ORDER_CREATED, order);
            return 1;
        }

        public async Task<OrderModel> GetOrderById(Guid id)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get order. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new OrderModel();
            }
            var order = await response.Content.ReadFromJsonAsyncNewtonsoft<OrderModel>();
            InVokeOnDatabaseChange(DatabaseChangedType.ORDER_RETRIEVED, order);
            return order;
           
        }

        public async Task<List<OrderModel>> GetAllOrders(int page = int.MinValue)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get order. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new List<OrderModel>();
            }
            var orders = await response.Content.ReadFromJsonAsyncNewtonsoft<List<OrderModel>>();
            InVokeOnDatabaseChange(DatabaseChangedType.ORDERS_RETRIEVED, orders);
            return orders;
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
            var response = await _httpClient.PutAsJsonAsyncNewtonsoft($"{ApiBaseUrl}/order", order);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to update order. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.ORDER_UPDATED, order);
            return 1;
        }

        public async Task<int> DeleteOrder(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/order/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to delete order. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.ORDER_DELETED, id);
            return 1;
        }

        public async Task<List<OrderModel>> SearchOrders(string search, int page = int.MinValue)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order/search?search={search}&page={page}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Failed to get orders. StatusCode: {response.StatusCode}, Content: {errorContent}");
                    return new List<OrderModel>();
                }
                List<OrderModel> orders = await response.Content.ReadFromJsonAsyncNewtonsoft<List<OrderModel>>();
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
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/order/count");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to count order. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            int count = await response.Content.ReadFromJsonAsyncNewtonsoft<int>();
            InVokeOnDatabaseChange(DatabaseChangedType.ORDER_COUNT_CHANGED, count);
            return count;  
        }
        #endregion

        #region FileModel CRUD Implementation
        public async Task<int> CreateFile(FileModel file)
        {
            var response = await _httpClient.PostAsJsonAsyncNewtonsoft($"{ApiBaseUrl}/file", file);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to cretae file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.FILE_CREATED, file);
            return 1;
        }

        public async Task<FileModel> GetFileById(Guid id)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new FileModel();
            }
            var file = await response.Content.ReadFromJsonAsyncNewtonsoft<FileModel>();
            InVokeOnDatabaseChange(DatabaseChangedType.FILE_RETRIEVED, file);
            return file;
     
        }

        public async Task<List<FileModel>> GetAllFilesByOrderId(Guid id)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/order/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get all file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new List<FileModel>();
            }
            var files = await response.Content.ReadFromJsonAsyncNewtonsoft<List<FileModel>>();
            InVokeOnDatabaseChange(DatabaseChangedType.FILES_RETRIEVED, files);
            return files;
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
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/order/{id}/withoutcontent");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get files without content. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new List<FileModel>();
            }
            var files = await response.Content.ReadFromJsonAsyncNewtonsoft<List<FileModel>>();
            InVokeOnDatabaseChange(DatabaseChangedType.FILES_RETRIEVED, files);
            return files;
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
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get all file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return new List<FileModel>();
            }
            var files = await response.Content.ReadFromJsonAsyncNewtonsoft<List<FileModel>>();
            InVokeOnDatabaseChange(DatabaseChangedType.FILES_RETRIEVED, files);
            return files;
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
            var response = await _httpClient.PutAsJsonAsyncNewtonsoft($"{ApiBaseUrl}/file", file);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to update file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.FILE_UPDATED, file);
            return 1;
        }

        public async Task<int> DeleteFile(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/file/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to delete file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            InVokeOnDatabaseChange(DatabaseChangedType.FILE_DELETED, id);
            return 1;
        }

        public async Task<string> GetFileContentSize(Guid id)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/content/size/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to get count size file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return string.Empty;
            }
            string size = await response.Content.ReadAsStringAsync();
            InVokeOnDatabaseChange(DatabaseChangedType.FILE_SIZE, size);
            return size;
        }

        public async Task<int> CountFiles()
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/file/count");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Failed to count file. StatusCode: {response.StatusCode}, Content: {errorContent}");
                return 0;
            }
            int count = await response.Content.ReadFromJsonAsyncNewtonsoft<int>();
            InVokeOnDatabaseChange(DatabaseChangedType.FILE_COUNT_CHANGED, count);
            return count;
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

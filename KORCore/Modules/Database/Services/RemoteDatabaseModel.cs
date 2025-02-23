using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Utility;
using KORCore.Modules.Remote.Utility;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace KORCore.Modules.Database.Services
{
    public class RemoteDatabaseModel : IRemoteDatabase
    {
        private static string ApiBaseUrl = string.Empty;
        private readonly HttpClient _httpClient;

        public RemoteDatabaseModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("kor_connection_client");
        }

        public string GetConnectedUrl()
        {
            return ApiBaseUrl;
        }

        public void SetUrl(string url)
        {
            ApiBaseUrl = url + "/api";
            _httpClient.BaseAddress = new Uri(GetConnectedUrl());
        }
        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        #region CustomerModel CRUD Implementation
        public async Task<int> CreateCustomer(CustomerModel customer)
        {
            var response = await _httpClient.PostAsync("api/customers", new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json"));
            return await DeserializeResponse<int>(response);
        }

        public async Task<CustomerModel> GetCustomerById(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/customers/{id}");
            return await DeserializeResponse<CustomerModel>(response);
        }

        public async Task<List<CustomerModel>> GetAllCustomers(int page = 0)
        {
            var response = await _httpClient.GetAsync($"api/customers?page={page}");
            return await DeserializeResponse<List<CustomerModel>>(response);
        }

        public async IAsyncEnumerable<CustomerModel> GetAllCustomersAsStream([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync("api/customers/stream", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();
            foreach (var customer in JsonConvert.DeserializeObject<List<CustomerModel>>(json))
            {
                yield return customer;
            }
        }

        public async Task<int> UpdateCustomer(CustomerModel customer)
        {
            var response = await _httpClient.PutAsync("api/customers", new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json"));
            return await DeserializeResponse<int>(response);
        }

        public async Task<int> DeleteCustomer(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/customers/{id}");
            return await DeserializeResponse<int>(response);
        }
        #endregion

        #region OrderModel CRUD Implementation
        public async Task<int> CreateOrder(OrderModel order)
        {
            var response = await _httpClient.PostAsync("api/orders", new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json"));
            return await DeserializeResponse<int>(response);
        }

        public async Task<OrderModel> GetOrderById(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/orders/{id}");
            return await DeserializeResponse<OrderModel>(response);
        }

        public async Task<List<OrderModel>> GetAllOrders(int page = 0)
        {
            var response = await _httpClient.GetAsync($"api/orders?page={page}");
            return await DeserializeResponse<List<OrderModel>>(response);
        }

        public async IAsyncEnumerable<OrderModel> GetAllOrdersAsStream([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync("api/orders/stream", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();
            foreach (var order in JsonConvert.DeserializeObject<List<OrderModel>>(json))
            {
                yield return order;
            }
        }

        public async Task<int> UpdateOrder(OrderModel order)
        {
            var response = await _httpClient.PutAsync("api/orders", new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json"));
            return await DeserializeResponse<int>(response);
        }

        public async Task<int> DeleteOrder(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/orders/{id}");
            return await DeserializeResponse<int>(response);
        }
        #endregion

        #region FileModel CRUD Implementation
        public async Task<int> CreateFile(FileModel file)
        {
            var response = await _httpClient.PostAsync("api/files", new StringContent(JsonConvert.SerializeObject(file), Encoding.UTF8, "application/json"));
            return await DeserializeResponse<int>(response);
        }

        public async Task<FileModel> GetFileById(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/files/{id}");
            return await DeserializeResponse<FileModel>(response);
        }

        public async Task<List<FileModel>> GetAllFiles()
        {
            var response = await _httpClient.GetAsync("api/files");
            return await DeserializeResponse<List<FileModel>>(response);
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesAsStream([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync("api/files/stream", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();
            foreach (var file in JsonConvert.DeserializeObject<List<FileModel>>(json))
            {
                yield return file;
            }
        }

        public async Task<int> UpdateFile(FileModel file)
        {
            var response = await _httpClient.PutAsync("api/files", new StringContent(JsonConvert.SerializeObject(file), Encoding.UTF8, "application/json"));
            return await DeserializeResponse<int>(response);
        }

        public async Task<int> DeleteFile(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/files/{id}");
            return await DeserializeResponse<int>(response);
        }
        #endregion



        #region CustomerModel CRUD Implementation
        public async IAsyncEnumerable<CustomerModel> SearchCustomerAsStream(string search, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"api/customers/search?query={search}", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();
            foreach (var customer in JsonConvert.DeserializeObject<List<CustomerModel>>(json))
            {
                yield return customer;
            }
        }

        public async Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue)
        {
            var response = await _httpClient.GetAsync($"api/customers/search?query={search}&page={page}");
            return await DeserializeResponse<List<CustomerModel>>(response);
        }

        public async Task<int> CountCustomers()
        {
            var response = await _httpClient.GetAsync("api/customers/count");
            return await DeserializeResponse<int>(response);
        }
        #endregion

        #region OrderModel CRUD Implementation
        public async Task<List<OrderModel>> SearchOrders(string search, int page = int.MinValue)
        {
            var response = await _httpClient.GetAsync($"api/orders/search?query={search}&page={page}");
            return await DeserializeResponse<List<OrderModel>>(response);
        }
        public async IAsyncEnumerable<OrderModel> SearchOrdersAsStream(string search, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"api/orders/search/stream?query={search}", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();
            foreach (var order in JsonConvert.DeserializeObject<List<OrderModel>>(json))
            {
                yield return order;
            }
        }
        public async Task<int> CountOrders()
        {
            var response = await _httpClient.GetAsync("api/orders/count");
            return await DeserializeResponse<int>(response);
        }
        #endregion

        #region FileModel CRUD Implementation
        public async Task<List<FileModel>> GetAllFilesByOrderId(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/files/order/{id}");
            return await DeserializeResponse<List<FileModel>>(response);
        }

        public async Task<List<FileModel>> GetFilesByOrderIdWithOutContent(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/files/order/{id}/without-content");
            return await DeserializeResponse<List<FileModel>>(response);
        }

        public async Task<string> GetFileContentSize(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/files/file/{id}/size");
            return await DeserializeResponse<string>(response);
        }

        public async IAsyncEnumerable<FileModel> GetAllFilesByOrderIdAsStream(Guid id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"api/files/order/{id}/stream", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();
            foreach (var file in JsonConvert.DeserializeObject<List<FileModel>>(json))
            {
                yield return file;
            }
        }
        public async IAsyncEnumerable<FileModel> GetFilesByOrderIdWithOutContentAsStream(Guid id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"api/files/order/{id}/without-content/stream", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();
            foreach (var file in JsonConvert.DeserializeObject<List<FileModel>>(json))
            {
                yield return file;
            }
        }

        public async Task<int> CountFiles()
        {
            var response = await _httpClient.GetAsync("api/files/count");
            return await DeserializeResponse<int>(response);
        }
        #endregion

        #region Database Export/Import Operations
        public Task ExportDatabaseToJson(string filePath, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            return Task.CompletedTask;
        }

        public Task ImportDatabaseFromJson(Stream jsonStream, Action<float> progressCallback = null)
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}

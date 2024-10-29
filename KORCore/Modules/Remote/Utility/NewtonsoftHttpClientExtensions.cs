using Newtonsoft.Json;
using System.Text;


namespace KORCore.Modules.Remote.Utility
{
    //base code: https://makolyte.com/csharp-newtonsoft-extension-methods-for-httpclient/
    public static class NewtonsoftHttpClientExtensions
    {
        public static async Task<T> GetFromJsonAsyncNewtonsoft<T>(this HttpClient httpClient, string uri, JsonSerializerSettings settings = null, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidParamsNewtonsoft(httpClient, uri);

            var response = await httpClient.GetAsync(uri, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsyncNewtonsoft<T>(this HttpClient httpClient, string uri, T value, JsonSerializerSettings settings = null, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidParamsNewtonsoft(httpClient, uri);

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var json = JsonConvert.SerializeObject(value, settings);

            var response = await httpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);

            response.EnsureSuccessStatusCode();

            return response;
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsyncNewtonsoft<T>(this HttpClient httpClient, string uri, T value, JsonSerializerSettings settings = null, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidParamsNewtonsoft(httpClient, uri);

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            }

            var json = JsonConvert.SerializeObject(value, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(uri, content, cancellationToken);

            response.EnsureSuccessStatusCode();

            return response;
        }

        public static async Task<T> ReadFromJsonAsyncNewtonsoft<T>(this HttpContent content, JsonSerializerSettings settings = null, CancellationToken cancellationToken = default)
        {
            var json = await content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        private static void ThrowIfInvalidParamsNewtonsoft(HttpClient httpClient, string uri)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Can't be null or empty", nameof(uri));
            }
        }
    }
}

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadManager
{
    /* public static partial class DownloadManager
     {
         static HttpClient Client = new HttpClient();

         public static void UseCustomHttpClient(HttpClient client)
         {
             if (client is null)
             {
                 throw new ArgumentNullException($"The {nameof(client)} can't be null.");
             }

             Client.Dispose();
             Client = null;
             Client = client;
         }

         public static async Task<string> DownloadAsync(
             string file,
             string url,
             IProgress<double> progress = default(IProgress<double>),
             CancellationToken token = default(CancellationToken))
         {
             var path = PlatformFolder();

             Directory.CreateDirectory(Path.GetDirectoryName(path));

             using (var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
             {
                 if (!response.IsSuccessStatusCode)
                     throw new Exception($"Error in download: {response.StatusCode}");

                 var total = response.Content.Headers.ContentLength ?? -1L;

                 using (var streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                 {
                     var totalRead = 0L;
                     var buffer = new byte[2048];
                     var isMoreToRead = true;
                     var fileWriteTo = Path.Combine(path, file);
                     var output = new FileStream(fileWriteTo, FileMode.Create);
                     do
                     {
                         token.ThrowIfCancellationRequested();

                         var read = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length, token);

                         if (read == 0)
                             isMoreToRead = false;

                         else
                         {
                             await output.WriteAsync(buffer, 0, read);

                             totalRead += read;

                             progress.Report((totalRead * 1d) / (total * 1d) * 100);
                         }

                     } while (isMoreToRead);

                     output.Close();
                     return fileWriteTo;
                 }
             }
         }
     }*/

    public static partial class DownloadManager
    {
        static HttpClient Client = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 10 }); // Allow multiple connections

        public static void UseCustomHttpClient(HttpClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException($"The {nameof(client)} can't be null.");
            }

            Client.Dispose();
            Client = null;
            Client = client;
        }

        public static async Task<string> DownloadAsync(
            string file,
            string url,
            IProgress<double> progress = default(IProgress<double>),
            CancellationToken token = default(CancellationToken))
        {
            var path = PlatformFolder();

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error in download: {response.StatusCode}");
                }

                var total = response.Content.Headers.ContentLength ?? -1L;

                using (var streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var output = new FileStream(Path.Combine(path, file), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;

                    do
                    {
                        token.ThrowIfCancellationRequested();

                        var read = await streamToReadFrom.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);

                        if (read == 0)
                            isMoreToRead = false;
                        else
                        {
                            await output.WriteAsync(buffer, 0, read, token).ConfigureAwait(false);
                            totalRead += read;

                            if (total > 0 && progress != null)
                            {
                                progress.Report((totalRead * 1d) / (total * 1d) * 100);
                            }
                        }

                    } while (isMoreToRead);

                    return Path.Combine(path, file);
                }
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Anf.Networks
{
    public class HttpClientAdapter : INetworkAdapter
    {
        public HttpClient HttpClient { get; }

        public HttpClientAdapter(HttpClient httpClient)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        public Task<HttpResponseMessage> GetMessageAsync(RequestSettings settings)
        {

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Debug.Assert(HttpClient != null);
            var req = new HttpRequestMessage();
            req.RequestUri = new Uri(settings.Address);
            if (settings.Method != null)
            {
                if (string.Equals("POST", settings.Method, StringComparison.OrdinalIgnoreCase))
                {
                    req.Method = HttpMethod.Post;
                }
                else if (string.Equals("PUT", settings.Method, StringComparison.OrdinalIgnoreCase))
                {
                    req.Method = HttpMethod.Put;
                }
                else
                {
                    req.Method = HttpMethod.Get;
                }
            }
            if (!string.IsNullOrEmpty(settings.Accept))
            {
                req.Headers.Add("Accept", settings.Accept);
            }
            if (!string.IsNullOrEmpty(settings.Host))
            {
                req.Headers.Host = settings.Host;
            }
            if (!string.IsNullOrEmpty(settings.Referrer))
            {
                req.Headers.Referrer = new Uri(settings.Referrer);
            }
            var contentType = "application/x-www-form-urlencoded";
            if (settings.Headers != null)
            {
                foreach (var item in settings.Headers)
                {
                    if (string.Equals("Content-Type",item.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = item.Value;
                        continue;
                    }
                    req.Headers.Add(item.Key, item.Value);
                }
            }
            if (settings.Data != null)
            {
                req.Content = new StreamContent(settings.Data);
                req.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }
            return HttpClient.SendAsync(req);
        }
        public async Task<Stream> GetStreamAsync(RequestSettings settings)
        {
            var rep = await GetMessageAsync(settings);
            return await rep.Content.ReadAsStreamAsync();
        }
    }
}

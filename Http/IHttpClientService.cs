using Blackwater.Core.Http.Models;
using System.Net.Http.Headers;

namespace Blackwater.Core.Http
{
    public interface IHttpClientService : IDisposable
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Uri BaseAddress { get; set; }
        TimeSpan Timeout { get; set; }
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri, HttpContent content, CancellationToken cancellationToken = default);
        Task<string> GetStringAsync(string requestUri, CancellationToken cancellationToken = default);
        Task<byte[]> GetByteArrayAsync(string requestUri, CancellationToken cancellationToken = default);
        Task<Stream> GetStreamAsync(string requestUri, CancellationToken cancellationToken = default);
        void UpdateDefaultRequestHeaders(Action<HttpRequestHeaders> update);
        void SetProxies(List<ProxyInfo> proxies);
    }
}

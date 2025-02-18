using Blackwater.Core.Http.Models;
using System.Net;
using System.Net.Http.Headers;

namespace Blackwater.Core.Http
{

    // this HttpClientService is just a wrapper around .NET's HttpClient for a pretty niche purpose
    // as blackwater is just a service provided around ROBLOX's web APIs its pretty critical that we dont get rate limited / stop providing a smooth service because we are blindly sending HTTP requests
    // essentially this service works exactly like the httpClient with an easy syntax (httpClientService.GetAsync("url") etc) that automatically rotates between proxies after reaching rate limits

    public class HttpClientService : IHttpClientService
    {
        private List<HttpClient> _httpClients;
        private int _currentProxyIndex;
        private readonly Lock _lock = new();

        private readonly int _maxRetries;
        private readonly int _retryDelayMs;

        public Uri BaseAddress
        {
            get
            {
                var baseAddress = _httpClients[_currentProxyIndex].BaseAddress;
                return baseAddress ?? throw new InvalidOperationException("BaseAddress is null");
            }
            set
            {
                foreach (var client in _httpClients)
                {
                    client.BaseAddress = value;
                }
            }
        }


        public TimeSpan Timeout
        {
            get => _httpClients[_currentProxyIndex].Timeout;
            set
            {
                foreach (var client in _httpClients)
                {
                    client.Timeout = value;
                }
            }
        }

        public HttpRequestHeaders DefaultRequestHeaders => _httpClients[_currentProxyIndex].DefaultRequestHeaders;

        public HttpClientService(HttpClientOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            if (options.Proxies is null || options.Proxies.Count == 0)
            {
                throw new ArgumentException("Proxies list cannot be empty.", nameof(options.Proxies));
            }

            _maxRetries = options.MaxRetries;
            _retryDelayMs = options.RetryDelayMs;

            _httpClients = [];
            _currentProxyIndex = 0;

            foreach (var proxy in options.Proxies)
            {
                _httpClients.Add(CreateHttpClientForProxy(proxy, options.TimeoutSeconds));
            }
        }

        private static HttpClient CreateHttpClientForProxy(ProxyInfo proxy, int timeoutSeconds)
        {
            var webProxy = new WebProxy(new Uri("http://" + proxy.Address))
            {
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(proxy.Username, proxy.Password)
            };

            var handler = new HttpClientHandler
            {
                Proxy = webProxy,
                PreAuthenticate = true,
                UseDefaultCredentials = false
            };

            var client = new HttpClient(handler, disposeHandler: true)
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            return client;
        }

        private HttpClient GetCurrentClient()
        {
            lock (_lock)
            {
                return _httpClients[_currentProxyIndex];
            }
        }

        private void RotateProxy()
        {
            lock (_lock)
            {
                _currentProxyIndex = (_currentProxyIndex + 1) % _httpClients.Count;
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            int attempts = 0;

            while (attempts < _maxRetries)
            {
                attempts++;

                try
                {
                    var clonedRequest = CloneHttpRequestMessage(request);
                    var client = GetCurrentClient();
                    var response = await client.SendAsync(clonedRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }

                    if ((int)response.StatusCode == 429)
                    {
                        RotateProxy();
                        await Task.Delay(_retryDelayMs, cancellationToken);
                        continue;
                    }

                    if (response.StatusCode == HttpStatusCode.Forbidden ||
                        response.StatusCode == HttpStatusCode.BadRequest ||
                        response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return response;
                    }

                    RotateProxy();
                    await Task.Delay(_retryDelayMs, cancellationToken);
                }
                catch
                {
                    RotateProxy();
                    await Task.Delay(_retryDelayMs, cancellationToken);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = "Recurring exception after retry policy"
            };
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default) =>
            SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default) =>
            SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, cancellationToken);

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default) =>
            SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content }, cancellationToken);

        public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default) =>
            SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);

        public Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default) =>
            SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content }, cancellationToken);

        public Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return SendAsync(request, cancellationToken);
        }

        public async Task<string> GetStringAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            var response = await GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<byte[]> GetByteArrayAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            var response = await GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        public async Task<Stream> GetStreamAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            var response = await GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }

        public void UpdateDefaultRequestHeaders(Action<HttpRequestHeaders> update)
        {
            lock (_lock)
            {
                foreach (var client in _httpClients)
                {
                    update(client.DefaultRequestHeaders);
                }
            }
        }

        public void SetProxies(List<ProxyInfo> proxies)
        {
            lock (_lock)
            {
                foreach (var client in _httpClients)
                {
                    client.Dispose();
                }

                if (proxies is null)
                {
                    throw new ArgumentNullException(nameof(proxies));
                }

                if (proxies.Count == 0)
                {
                    throw new ArgumentException("Proxies list cannot be empty.", nameof(proxies));
                }

                _httpClients = [];

                foreach (var proxy in proxies)
                {
                    _httpClients.Add(CreateHttpClientForProxy(proxy, (int)Timeout.TotalSeconds));
                }

                _currentProxyIndex = 0;
            }
        }

        private static HttpRequestMessage CloneHttpRequestMessage(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content,
                Version = request.Version
            };

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    clone.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clone;
        }

        public void Dispose()
        {
            foreach (var client in _httpClients)
            {
                client.Dispose();
            }
        }
    }
}

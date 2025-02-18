namespace Blackwater.Core.Http.Models
{
    public class HttpClientOptions
    {
        public List<ProxyInfo> Proxies { get; set; } = ProxyProvider.GetProxies();
        public int MaxRetries { get; set; } = 10;
        public int RetryDelayMs { get; set; } = 3000;
        public int TimeoutSeconds { get; set; } = 30;
    }
}

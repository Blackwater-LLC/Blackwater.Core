namespace Blackwater.Core.Http.Models
{
    public static class ProxyProvider
    {
        // as this is open sourced and a fork of the actual provider, you would probably want an actual provider fetching the proxies from your proxy provider or just hard code them in dev
        // this works as is with the httpClientService as long as a list of proxyInfo objects is returned
        public static List<ProxyInfo> GetProxies() =>
        [
            new("0.0.0.0:0000", "user", "password"),
        ];
    }
}

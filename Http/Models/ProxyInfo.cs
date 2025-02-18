namespace Blackwater.Core.Http.Models
{
    public class ProxyInfo(string address, string username, string password)
    {
        public string Address { get; set; } = address;
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;
    }
}

using Grpc.Core;
using Microsoft.AspNetCore.DataProtection;

namespace IT.WebHost.Api.Models
{
    public class ServicesSettings
    {
        public ServicesEndpointSettings Settings { get; set; }
    }

    public class ServicesEndpointSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsSecure { get; set; }

        public Uri ToUri() =>
            new Uri($"{(IsSecure ? "https" : "http")}://{Host}:{Port}");

        public ChannelCredentials ToChannelCredentials() =>
            IsSecure ? ChannelCredentials.SecureSsl : ChannelCredentials.Insecure;
    }
}

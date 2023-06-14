using DWOS.Utilities;
using System;
using System.Net;
using System.Net.Http;

namespace DWOS.Services
{
    internal static class HttpClientSingleton
    {
        private static readonly Lazy<HttpClient> _clientLazy = new Lazy<HttpClient>(NewHttpClient);

        public static HttpClient Instance => _clientLazy.Value;

        private static HttpClient NewHttpClient()
        {
            var logging = ServiceContainer.Resolve<ILogService>();

            var handler = new TimerMessageHandler(logging)
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            return new HttpClient(handler);
        }

    }
}

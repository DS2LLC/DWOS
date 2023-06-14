using NLog;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Logs all API requests using NLog.
    /// </summary>
    internal class RequestLogHandler : DelegatingHandler
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.Info($"REST request received: {request.Method} {request.RequestUri.AbsolutePath}");
            return base.SendAsync(request, cancellationToken);
        }
    }
}

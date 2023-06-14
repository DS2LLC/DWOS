using ModernHttpClient;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Native Http message handler that logs elapsed time.
    /// </summary>
    internal class TimerMessageHandler : NativeMessageHandler
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="ILogService"/> instance.
        /// </summary>
        public ILogService Logging
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerMessageHandler"/> class.
        /// </summary>
        /// <param name="logging"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logging"/> is null.
        /// </exception>
        public TimerMessageHandler(ILogService logging)
        {
            if (logging == null)
            {
                throw new ArgumentNullException(nameof(logging));
            }

            Logging = logging;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = null;
            try
            {
                stopwatch = Stopwatch.StartNew();
                return await base.SendAsync(request, cancellationToken);
            }
            finally
            {
                if (stopwatch != null)
                {
                    stopwatch.Stop();
                    Logging.LogElapsedTime(request.RequestUri, stopwatch.Elapsed);
                }
            }
        }

        #endregion
    }
}

using System;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines logging functionality.
    /// </summary>
    /// <remarks>
    /// This interface is for general logging functionality that may access
    /// the DWOS server. <see cref="IDWOSLoggingService"/> always access the
    /// server.
    /// </remarks>
    public interface ILogService
    {
        /// <summary>
        /// Logs an error and conditionally shows a toast.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exception">The exception..</param>
        /// <param name="context">Context to use for the toast.</param>
        /// <param name="toast">
        /// <c>true</c> if the service should show a toast; otherwise, <c>false</c>
        /// </param>
        void LogError(string message, Exception exception = default(Exception), object context = default(object), bool toast = default(bool));

        /// <summary>
        /// Logs info asynchronously.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task LogInfoAsync(string message, object context = default(object));

        /// <summary>
        /// Logs the elapsed time for a server request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="elapsedTime"></param>
        void LogElapsedTime(Uri requestUri, TimeSpan elapsedTime);
    }
}

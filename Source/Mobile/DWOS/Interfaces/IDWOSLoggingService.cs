using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for logging API access.
    /// </summary>
    /// <remarks>
    /// This interface is for logging directly to the DWOS server.
    /// <see cref="ILogService"/> defines general logging functionality.
    /// </remarks>
    public interface IDWOSLoggingService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Logs the information asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="userProfile">The user profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        /// <exception cref="System.InvalidOperationException">message cannot be null or empty</exception>
        Task<ResponseBase> LogInfoAsync(string message, UserProfileInfo userProfile, CancellationToken cancellationToken = default(CancellationToken));
    }
}

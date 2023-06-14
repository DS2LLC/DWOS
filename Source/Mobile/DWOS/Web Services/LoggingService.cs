using DWOS.Services;
using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Implements <see cref="IDWOSLoggingService"/>.
    /// </summary>
    internal class DWOSLoggingService : ServiceBase, IDWOSLoggingService
    {
        public DWOSLoggingService(ILogService logService) : base(logService)
        {
        }

        /// <summary>
        /// Logs the information asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="userProfile">The user profile.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        /// <exception cref="System.InvalidOperationException">message cannot be null or empty</exception>
        public async Task<ResponseBase> LogInfoAsync(string message, UserProfileInfo userProfile, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userProfile == null)
                throw new ArgumentNullException("userProfile");

            if (string.IsNullOrEmpty(message))
                throw new InvalidOperationException("message cannot be null or empty");

            var request = new LogMessageRequest
            {
                Message = message,
                UserId = userProfile.UserId,
                UserProfile = userProfile
            };

            var response = await PostAsync<ResponseBase>("/logging/add", request, cancellationToken);
            return response;
        }
    }
}

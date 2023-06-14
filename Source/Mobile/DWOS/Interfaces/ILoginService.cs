using DWOS.Services.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines login functionality.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Performs login asynchronously.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">pin</exception>
        Task<UserProfileResponse> LogInUserAsync(string pin, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets information about a user asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">pin</exception>
        Task<UserResponse> GetUserAsync(UserRequest request, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets application settings asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        Task<ApplicationSettingsResponse> GetApplicationSettings(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Verifies the server located at <see cref="RootUrl"/> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> VerifyServer(CancellationToken cancellationToken = default(CancellationToken));
    }
}

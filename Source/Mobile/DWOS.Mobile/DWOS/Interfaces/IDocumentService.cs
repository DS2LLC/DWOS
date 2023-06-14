using DWOS.Services.Messages;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for document API access.
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Gets or sets the root URL for the API.
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Gets a document asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The document</returns>
        /// <exception cref="ArgumentNullException">request is null.</exception>
        Task<DocumentResponse> GetDocumentAsync(DocumentRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}

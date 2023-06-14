using DWOS.Services.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DWOS.Services
{
    /// <summary>
    /// Implements <see cref="IDocumentService"/>.
    /// </summary>
    internal class DocumentService : ServiceBase, IDocumentService
    {
        public DocumentService(ILogService logService) : base(logService)
        {
        }

        public Task<DocumentResponse> GetDocumentAsync(DocumentRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return GetAsync<DocumentResponse>($"/documents/get/?revisionId={request.RevisionId}", cancellationToken);
        }
    }
}

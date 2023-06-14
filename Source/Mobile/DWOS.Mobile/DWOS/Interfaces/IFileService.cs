using DWOS.Services.Messages;
using System.Threading.Tasks;

namespace DWOS
{
    /// <summary>
    /// Defines functionality for document/media API access.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Downloads a document asynchronously.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<ResponseBase> Download(DocumentInfo document);

        /// <summary>
        /// Gets the expected path for a document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        string GetPath(DocumentInfo document);

        /// <summary>
        /// Downloads media asynchronously.
        /// </summary>
        /// <param name="media"></param>
        /// <returns></returns>
        Task<ResponseBase> Download(MediaSummary media);

        /// <summary>
        /// Gets the expected path for media.
        /// </summary>
        /// <param name="media"></param>
        /// <returns></returns>
        string GetPath(MediaSummary media);
    }
}

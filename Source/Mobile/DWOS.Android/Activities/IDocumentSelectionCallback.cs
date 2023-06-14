using DWOS.Services.Messages;

namespace DWOS.Android
{
    /// <summary>
    /// Provides functionality to handle document selection.
    /// </summary>
    public interface IDocumentSelectionCallback
    {
        /// <summary>
        /// Called when the user selects a document.
        /// </summary>
        /// <param name="selectedDocument"></param>
        void OnDocumentInfoSelected(DocumentInfo selectedDocument);

        /// <summary>
        /// Called when the user selects media.
        /// </summary>
        /// <param name="selectedMedia"></param>
        void OnMediaSummarySelected(MediaSummary selectedMedia);
    }


}
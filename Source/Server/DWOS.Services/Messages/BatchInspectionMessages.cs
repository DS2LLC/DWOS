namespace DWOS.Services.Messages
{
    /// <summary>
    /// Client request to complete a batch inspection.
    /// </summary>
    public class BatchInspectionSaveCompletedRequest : RequestBase
    {
        /// <summary>
        /// Gets or sets the batch ID for this instance.
        /// </summary>
        public int BatchId { get; set; }
    }
}

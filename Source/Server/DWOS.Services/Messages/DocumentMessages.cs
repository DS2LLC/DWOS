namespace DWOS.Services.Messages
{
    /// <summary>
    /// Represents information about a document.
    /// </summary>
    public class DocumentInfo
    {
        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current revision ID for this instance.
        /// </summary>
        public int CurrentRevisionId { get; set; }

        /// <summary>
        /// Gets or sets the document info ID for this instance.
        /// </summary>
        public int DocumentInfoID { get; set; }

        /// <summary>
        /// Gets or sets the media type for this instance.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets the document type for this instance.
        /// </summary>
        /// <value>
        /// General category that the document belongs to. Examples: "Order",
        /// "Process", "ControlInspection".
        /// </value>
        public string DocumentType { get; set; }
    }

    /// <summary>
    /// Represents detailed information about a document.
    /// </summary>
    public class DocumentDetail
    {
        /// <summary>
        /// Gets or sets the document revision ID for this instance.
        /// </summary>
        public int DocumentRevisionId { get; set; }

        /// <summary>
        /// Gets or sets the document info ID for this instance.
        /// </summary>
        public int DocumentInfoID { get; set; }

        /// <summary>
        /// Gets or sets the media type for this instance.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this instance has compressed
        /// data.
        /// </summary>
        /// <remarks>
        /// This is not automatically set.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance contains compressed data;
        /// otherwise, false</c>.
        /// </value>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Gets or sets the binary data for this instance.
        /// </summary>
        public byte[] DocumentData { get; set; }
    }

    /// <summary>
    /// Server response containing detailed document information.
    /// </summary>
    public class DocumentResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the document for this instance.
        /// </summary>
        public DocumentDetail Document { get; set; }
    }
}

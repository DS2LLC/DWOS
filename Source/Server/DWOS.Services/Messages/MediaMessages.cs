namespace DWOS.Services.Messages
{
    /// <summary>
    /// Server response containing media information.
    /// </summary>
    public class MediaDetailResponse : ResponseBase
    {
        /// <summary>
        /// Gets or sets the media information for this instance.
        /// </summary>
        public MediaInfo Media { get; set; }
    }

    /// <summary>
    /// Represents a piece of media.
    /// </summary>
    public class MediaInfo
    {
        /// <summary>
        /// Gets or sets the media ID for this instance.
        /// </summary>
        public int MediaId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file extension for this instance.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the file name for this instance.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the media data for this instance.
        /// </summary>
        public byte[] Media { get; set; }
    }

    /// <summary>
    /// Represents a summary for a piece of media.
    /// </summary>
    public class MediaSummary
    {
        /// <summary>
        /// Gets or sets the media ID for this instance.
        /// </summary>
        public int MediaId { get; set; }

        /// <summary>
        /// Gets or sets the name for this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file extension for this instance.
        /// </summary>
        public string FileExtension { get; set; }
    }
}

namespace DWOS.Portal.Models
{
    /// <summary>
    /// Represents data for a single file.
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// Gets or sets the base64 contents of the file.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the file.
        /// </summary>
        public string Type { get; set; }
    }
}
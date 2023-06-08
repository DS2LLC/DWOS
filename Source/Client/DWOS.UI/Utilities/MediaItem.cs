namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Represents a piece of media.
    /// </summary>
    public class MediaItem
    {
        #region Properties

        public int MediaId { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public byte[] Data { get; set; }

        #endregion
    }
}

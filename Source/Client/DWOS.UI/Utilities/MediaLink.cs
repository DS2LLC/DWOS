namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Represents a link between a <see cref="MediaItem"/> and something else
    /// in the application
    /// </summary>
    public class MediaLink
    {
        #region Properties

        public MediaItem Item { get; set; }

        public int ParentId { get; set; }

        public bool IsDefault { get; set; }

        #endregion
    }
}

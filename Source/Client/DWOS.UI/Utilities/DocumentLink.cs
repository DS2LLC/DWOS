namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Represents a link between a document and something else in the
    /// application.
    /// </summary>
    public class DocumentLink
    {
        #region Properties

        public int LinkId { get; set; }

        public int DocumentInfoId { get; set; }

        public int ParentId { get; set; }

        #endregion
    }
}

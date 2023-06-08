namespace DWOS.UI.Admin.PartManagerPanels
{
    /// <summary>
    /// Represents a note for a part process.
    /// </summary>
    internal sealed class ProcessNote
    {
        #region Properties

        public int PartProcessID
        {
            get;
            private set;
        }

        public string Notes
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessNote"/> class.
        /// </summary>
        /// <param name="partProcessID">The note's associated part process ID.</param>
        /// <param name="notes">Note content</param>
        public ProcessNote(int partProcessID, string notes)
        {
            PartProcessID = partProcessID;
            Notes = notes;
        }

        #endregion
    }
}

using DWOS.Data.Datasets;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Represents process alias information for import/export.
    /// </summary>
    public class ProcessAliasExport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the process alias.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the pop-up notes for the process alias.
        /// </summary>
        public string PopUpNotes { get; set; }

        /// <summary>
        /// Gets or sets the traveler notes for the process alias.
        /// </summary>
        public string TravelerNotes { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessAliasExport"/>
        /// class.
        /// </summary>
        public ProcessAliasExport() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessAliasExport"/>
        /// class.
        /// </summary>
        /// <param name="processAlias"></param>
        public ProcessAliasExport(Data.Datasets.ProcessesDataset.ProcessAliasRow processAlias)
        {
            this.Name = processAlias.Name;
            this.PopUpNotes = processAlias.IsPopUpNotesNull() ? null : processAlias.PopUpNotes;
            this.TravelerNotes = processAlias.IsTravelerNotesNull() ? null : processAlias.TravelerNotes;
        }

        /// <summary>
        /// Creates a row for this process alias.
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="process"></param>
        public void CreateRow(ProcessImporter importer, ProcessesDataset.ProcessRow process)
        {
            if (string.IsNullOrEmpty(Name))
            {
                importer.Issues.Add("Cannot import process alias: Name was not provided");
                return;
            }

            var processAlias = importer.Dataset.ProcessAlias.NewProcessAliasRow();
            processAlias.Name = this.Name;
            processAlias.ProcessRow = process;
            processAlias.PopUpNotes = this.PopUpNotes;
            processAlias.TravelerNotes = this.TravelerNotes;

            importer.Dataset.ProcessAlias.AddProcessAliasRow(processAlias);
        }

        #endregion
    }
}

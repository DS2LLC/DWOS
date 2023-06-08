using DWOS.Data.Datasets;
using System;

namespace DWOS.Data.Process
{
    /// <summary>
    /// Represents process inspection information for import/export.
    /// </summary>
    public class ProcessInspectionExport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the part inspection type ID for this process inspection.
        /// </summary>
        public int PartInspectionTypeID { get; set; }

        /// <summary>
        /// Gets or sets the name for this process inspection.
        /// </summary>
        public string PartInspectionName { get; set; }

        /// <summary>
        /// Gets or sets the step order for this process inspection.
        /// </summary>
        public int StepOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this process alias is listed on
        /// a COC by default.
        /// </summary>
        /// <value>
        /// <c>true</c> if the process alias should be on the COC by default;
        /// otherwise, <c>false</c>
        /// </value>
        public bool COCData { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessInspectionExport"/> class.
        /// </summary>
        public ProcessInspectionExport() { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ProcessInspectionExport"/> class.
        /// </summary>
        /// <param name="processInspection"></param>
        public ProcessInspectionExport(Data.Datasets.ProcessesDataset.ProcessInspectionsRow processInspection)
        {
            this.PartInspectionTypeID = processInspection.PartInspectionTypeID;
            this.PartInspectionName = processInspection.PartInspectionTypeRow.Name; //Get Name to compare later as we are not doing a deep clone
            this.StepOrder = processInspection.StepOrder;
            this.COCData = processInspection.COCData;
        }

        /// <summary>
        /// Creates a row for this process inspection.
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="process"></param>
        public void CreateRow(ProcessImporter importer, ProcessesDataset.ProcessRow process)
        {
            var partInspection = ((ProcessesDataset)process.Table.DataSet).PartInspectionType.FindByPartInspectionTypeID(PartInspectionTypeID);

            if (partInspection != null && partInspection.Name == PartInspectionName)
            {
                var processInspection = importer.Dataset.ProcessInspections.NewProcessInspectionsRow();
                processInspection.PartInspectionTypeRow = partInspection;
                processInspection.ProcessRow = process;
                processInspection.StepOrder = this.StepOrder;
                processInspection.COCData = this.COCData;

                importer.Dataset.ProcessInspections.AddProcessInspectionsRow(processInspection);
            }
            else
                importer.Issues.Add("Did not add process inspection {0} as it does not match existing inspections.".FormatWith(PartInspectionName));
        }

        #endregion
    }
}

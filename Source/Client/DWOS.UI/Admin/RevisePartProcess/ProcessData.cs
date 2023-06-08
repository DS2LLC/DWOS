using System;
using DWOS.Data;
using DWOS.Data.Datasets;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public class ProcessData
    {
        #region Properties

        public int ProcessId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string CurrentRevision { get; private set; }

        public string NextRevision { get; set; }

        #endregion

        #region Methods

        public static ProcessData From(ProcessesDataset.ProcessRow processRow)
        {
            if (processRow == null)
            {
                return null;
            }

            var revision = processRow.IsRevisionNull() ? String.Empty : processRow.Revision;

            var nextRevision = revision == "<None>"
                ? "A"
                : revision.Increment();

            return new ProcessData
            {
                ProcessId = processRow.ProcessID,
                Name = processRow.Name,
                Description = processRow.IsDescriptionNull() ? null : processRow.Description,
                CurrentRevision = revision,
                NextRevision = nextRevision
            };
        }

        #endregion
    }
}
using DWOS.Data.Datasets;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public class ProcessPackageData
    {
        #region Properties

        public int ProcessPackageId { get; private set; }

        public string Name { get; private set; }

        #endregion

        #region Methods

        public static ProcessPackageData From(ProcessPackageDataset.ProcessPackageRow processPackageRow)
        {
            if (processPackageRow == null)
            {
                return null;
            }

            return new ProcessPackageData
            {
                ProcessPackageId = processPackageRow.ProcessPackageID,
                Name = processPackageRow.Name
            };
        }

        #endregion
    }
}

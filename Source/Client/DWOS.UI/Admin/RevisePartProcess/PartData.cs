using System;
using System.Linq;
using DWOS.Data;
using DWOS.Data.Datasets;

namespace DWOS.UI.Admin.RevisePartProcess
{
    public class PartData
    {
        #region Properties

        public int PartId { get; private set; }

        public string CustomerName { get; private set; }

        public string PartName { get; private set; }

        public string Description { get; private set; }

        public string CurrentRevision { get; private set; }

        public string NextRevision { get; set; }

        public string DisplayText => $@"{CustomerName} - {PartName}";

        #endregion

        #region Methods

        public static PartData From(PartsDataset.PartRow partRow)
        {
            if (partRow == null)
            {
                return null;
            }

            var revision = partRow.IsRevisionNull() ? String.Empty : partRow.Revision;

            var nextRevision = revision == "<None>"
                ? "A"
                : revision.Increment();

            return new PartData
            {
                PartId = partRow.PartID,
                CustomerName = partRow.CustomerRow?.Name ?? GetCustomerName(partRow.CustomerID),
                Description = partRow.IsDescriptionNull() ? null : partRow.Description,
                PartName = partRow.Name,
                CurrentRevision = revision,
                NextRevision = nextRevision
            };
        }

        private static string GetCustomerName(int customerId)
        {
            using (var dtCustomer = new PartsDataset.CustomerDataTable())
            {
                using (var taCustomer = new Data.Datasets.PartsDatasetTableAdapters.CustomerTableAdapter())
                {
                    taCustomer.FillByCustomer(dtCustomer, customerId);
                }

                return dtCustomer.FirstOrDefault()?.Name;
            }
        }

        #endregion
    }
}

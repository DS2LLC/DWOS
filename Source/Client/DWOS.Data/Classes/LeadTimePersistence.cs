using System;
using DWOS.Data.Datasets;

namespace DWOS.Data
{
    /// <summary>
    /// <see cref="ILeadTimePersistence"/> implementation that accesses the database.
    /// </summary>
    public class LeadTimePersistence : ILeadTimePersistence
    {
        #region Properties

        /// <summary>
        /// Gets the dataset for this instance.
        /// </summary>
        public ScheduleDataset Data { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadTimePersistence"/> class.
        /// </summary>
        public LeadTimePersistence()
        {
            Data = new ScheduleDataset();

            using (var ta = new Datasets.ScheduleDatasetTableAdapters.d_ProcessCategoryTableAdapter())
            {
                ta.Fill(Data.d_ProcessCategory);
            }
        }

        #endregion

        #region ILeadTimePersistence Members

        public TimeSpan ReceivingRolloverTime => ApplicationSettings.Current.ReceivingRolloverTime;

        public double ShippingLeadTime => ApplicationSettings.Current.ShippingLeadTime;

        public double CocLeadTime => ApplicationSettings.Current.COCLeadTime;

        public double PartMarkingLeadTime => ApplicationSettings.Current.PartMarkingLeadTime;

        public bool CocEnabled => ApplicationSettings.Current.COCEnabled;

        public bool PartMarkingEnabled => ApplicationSettings.Current.PartMarkingEnabled;

        public decimal GetLeadTimeDays(int processId)
        {
            // Find, load, and cache process
            var process = Data.Process.FindByProcessID(processId);

            if (process == null)
            {
                using(var ta = new Datasets.ScheduleDatasetTableAdapters.ProcessTableAdapter() {ClearBeforeFill = false})
                    ta.FillBy(Data.Process, processId);

                process = Data.Process.FindByProcessID(processId);
            }

            // Retrieve process lead time for process
            if (process.d_ProcessCategoryRow != null && !process.d_ProcessCategoryRow.IsLeadTimeNull())
            {
                return process.d_ProcessCategoryRow.LeadTime;
            }

            return ApplicationSettings.Current.DefaultProcessLeadTime;
        }

        #endregion
    }
}

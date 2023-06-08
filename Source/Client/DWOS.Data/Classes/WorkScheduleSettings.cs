using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DWOS.Data
{
    public class WorkScheduleSettings
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        public const int SHIFTLENGTHHOURS = 8; //each shift is 8 hours
        public const int DAYSTARTTIMEHOURS = 6; //starts at 6 AM

        private int _maxNumberofShifts;

        public const string DEPT_COLUMN = "NextDept";
        public const string PROCESS_COLUMN = "NextProcess";
        public const string PROCESS_NAME_COLUMN = "NextProcessName";
        public const string ORDERPROCESS_COLUMN = "NextOrderProcess";
        public const string SCORE_COLUMN = "Score";

        #endregion

        #region Properties

        public int CustomerElevatedScore { get; set; }

        public int CustomerHighScore { get; set; }

        public int CustomerNormalScore { get; set; }

        public List<PriorityWeight> PriorityWeights { get; set; }

        public ScheduleDataset.WorkScheduleDataTable WorkSchedules { get; set; }

        public ScheduleDataset.CustomerSummaryDataTable Customers { get; set; }

        public int MaxNumberOfShifts
        {
            get
            {
                if(this._maxNumberofShifts < 1)
                {
                    var rows = this.WorkSchedules.Where(rr => rr.RowState != DataRowState.Deleted);
                    this._maxNumberofShifts = rows.Any() ? rows.Max(r => r.ShiftNumber) : 0;
                }

                return this._maxNumberofShifts;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has changes that need to be persisted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool HasChanges { get; set; }

        #endregion

        #region Methods

        public WorkScheduleSettings()
        {
            using(var ta = new CustomerSummaryTableAdapter())
            {
                this.Customers = new ScheduleDataset.CustomerSummaryDataTable();
                ta.Fill(this.Customers);
            }

            using(var ta = new WorkScheduleTableAdapter())
            {
                this.WorkSchedules = new ScheduleDataset.WorkScheduleDataTable();
                ta.Fill(this.WorkSchedules);
            }

            using(var ta = new d_CustomerOrderPriorityTableAdapter())
            {
                using(var op = new ScheduleDataset.d_CustomerOrderPriorityDataTable())
                {
                    ta.Fill(op);

                    this.CustomerHighScore = op.FindByCustomerOrderPriority("High").Score;
                    this.CustomerElevatedScore = op.FindByCustomerOrderPriority("Elevated").Score;
                    this.CustomerNormalScore = op.FindByCustomerOrderPriority("Normal").Score;
                }
            }

            using(var ta = new d_PriorityTableAdapter())
            {
                using(var op = new ScheduleDataset.d_PriorityDataTable())
                {
                    ta.Fill(op);

                    this.PriorityWeights = new List<PriorityWeight>();
                    foreach(ScheduleDataset.d_PriorityRow row in op)
                        this.PriorityWeights.Add(new PriorityWeight() { Name = row.PriorityID, Weight = row.PriorityScore});
                }
            }

            this.WorkSchedules.RowChanged += this.WorkSchedules_RowChanged;
        }

        public int NumberOfShifts(string department)
        {
            if(String.IsNullOrEmpty(department))
                return this.MaxNumberOfShifts;
            else
                return this.WorkSchedules.Select(this.WorkSchedules.DepartmentIDColumn.ColumnName + " = '" + department + "'").Length;
        }

        public int PartProductionCount(string department, int shift)
        {
            DataRow[] dr = this.WorkSchedules.Select(this.WorkSchedules.DepartmentIDColumn.ColumnName + " = '" + department + "' AND " + this.WorkSchedules.ShiftNumberColumn.ColumnName + " = " + shift);

            if(dr != null && dr.Length > 0)
                return (int)dr[0][this.WorkSchedules.PartProductionColumn];

            return 1500;
        }

        public int GetCustomerScore(int customerID)
        {
            return this.Customers.FindByCustomerID(customerID).Score;
        }

        public int GetPriorityWeight(string priorityName)
        {
            var p = this.PriorityWeights.FirstOrDefault(pr => pr.Name == priorityName);
            return p == null ? 0 : p.Weight;
        }

        public void SaveSettings()
        {
            _log.Info("Saving schedule settings.");
            this.HasChanges = false;

            //Update schedule settings
            using(var ta = new WorkScheduleTableAdapter())
            {
                ta.Update(this.WorkSchedules);
            }

            //update customer priority weights
            using(var ta = new d_CustomerOrderPriorityTableAdapter())
            {
                using(var op = new ScheduleDataset.d_CustomerOrderPriorityDataTable())
                {
                    ta.Fill(op);

                    op.FindByCustomerOrderPriority("High").Score = this.CustomerHighScore;
                    op.FindByCustomerOrderPriority("Elevated").Score = this.CustomerElevatedScore;
                    op.FindByCustomerOrderPriority("Normal").Score = this.CustomerNormalScore;

                    ta.Update(op);
                }
            }

            //update order priority weights
            using(var ta = new d_PriorityTableAdapter())
            {
                using(var table = new ScheduleDataset.d_PriorityDataTable())
                {
                    ta.Fill(table);

                    foreach(var kvp in this.PriorityWeights)
                        table.FindByPriorityID(kvp.Name).PriorityScore = kvp.Weight;

                    ta.Update(table);
                }
            }
        }

        #endregion

        #region Events

        private void WorkSchedules_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            this._maxNumberofShifts = 0;
        }

        #endregion

        #region PriorityWeight

        public class PriorityWeight
        {
            public string Name { get; set; }
            public int Weight { get; set; }
        }

        #endregion
    }
}


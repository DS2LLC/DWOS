using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ScheduleDatasetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ProcessScheduleSettings: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public WorkScheduleSettings Settings { get; set; }

        #endregion

        #region Methods

        public ProcessScheduleSettings()
        {
            this.InitializeComponent();
        }

        public void LoadSettings(WorkScheduleSettings settings, ScheduleDataset.WorkScheduleDataTable scheduleTable)
        {
            try
            {
                this.Settings = settings;
                this.grdWorkSchedule.SetDataBinding(scheduleTable, null);

                this.bsScheduleSettings.DataSource = settings;

                this.numCustPriorityEle.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(this.numCustPriorityEle), this.bsScheduleSettings, "CustomerElevatedScore", true));
                this.numCustPriorityHigh.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(this.numCustPriorityHigh), this.bsScheduleSettings, "CustomerHighScore", true));
                this.numCustPriorityNorm.DataBindings.Add(new Binding(ControlUtilities.GetBindingProperty(this.numCustPriorityNorm), this.bsScheduleSettings, "CustomerNormalScore", true));

                this.numOrderPriortyWeight.Value = ApplicationSettings.Current.ScheduleOrderPriorityMultiplier;
                this.numProcessWeight.Value = ApplicationSettings.Current.ScheduleProcessCountMultiplier;
                this.numLatenessWeight.Value = ApplicationSettings.Current.ScheduleDaysLateCountMultiplier;

                this.grdOrderPriorities.DataSource = settings.PriorityWeights;
                this.grdOrderPriorities.DataBind();
            }
            catch(Exception exc)
            {
                this.btnOK.Enabled = false;
                string errorMsg = "Error loading the settings.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        #endregion

        #region Events

        private void grdWorkSchedule_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                using(var ta = new d_DepartmentTableAdapter())
                {
                    var departments = new ScheduleDataset.d_DepartmentDataTable();
                    ta.Fill(departments);
                    
                    var list = DataUtilities.CreateValueList(departments, departments.DepartmentIDColumn.ColumnName, departments.DepartmentIDColumn.ColumnName, null);
                    e.Layout.Bands[0].Columns["DepartmentID"].ValueList = list;
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error initializing grid.";
                _log.Error(exc, errorMsg);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.grdWorkSchedule.UpdateData();
                this.bsScheduleSettings.EndEdit();

                this.grdOrderPriorities.UpdateData();

                ApplicationSettings.Current.ScheduleOrderPriorityMultiplier = Convert.ToInt32(this.numOrderPriortyWeight.Value);
                ApplicationSettings.Current.ScheduleProcessCountMultiplier = Convert.ToInt32(this.numProcessWeight.Value);
                ApplicationSettings.Current.ScheduleDaysLateCountMultiplier = Convert.ToInt32(this.numLatenessWeight.Value);
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving settings.";
                _log.Error(exc, errorMsg);
            }
        }

        private void grdWorkSchedule_CellChange(object sender, BeforeCellUpdateEventArgs e)
        {
            if(e.Cell.Column.Key == "ShiftNumber")
            {
                int shift = 0;

                if(int.TryParse(e.Cell.Text, out shift))
                {
                    if(shift > 0 && shift <= 3)
                    {
                        //all good
                    }
                    else
                    {
                        MessageBox.Show("Shift Number must be between 1 - 3.", About.ApplicationCompany, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else
                {
                    MessageBox.Show("Shift Number must be a number between 1 - 3.", About.ApplicationCompany, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }

            if (e.Cell.Column.Key == "PartProduction")
            {
                int processCount = 0;

                if (int.TryParse(e.Cell.Text, out processCount))
                {
                    if (processCount > 0)
                    {
                        //all good
                    }
                    else
                    {
                        MessageBox.Show("Process count must be a number greater than 0.", About.ApplicationCompany, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else
                {
                    MessageBox.Show("Process count must be a number greater than 0.", About.ApplicationCompany, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        #endregion
    }
}
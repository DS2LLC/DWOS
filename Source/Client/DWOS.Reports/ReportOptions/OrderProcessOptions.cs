using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data.Reports.ProcessPartsReportTableAdapters;
using DWOS.Reports.Properties;
using DWOS.Shared;
using NLog;

namespace DWOS.Reports.ReportOptions
{
    public partial class OrderProcessOptions : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        public OrderProcessOptions() { InitializeComponent(); }

        private void LoadProcesses()
        {
            try
            {
                var processes = new Data.Reports.ProcessPartsReport.ProcessSummaryDataTable();
                processes.Constraints.Clear();

                cboProcess.BeginUpdate();

                using(var taProcesses = new ProcessSummaryTableAdapter())
                {
                    if(chkActiveOnly.Checked)
                        taProcesses.FillProcessNamesActive(processes);
                    else
                        taProcesses.FillProcessNames(processes);
                }

                this.cboProcess.DataSource = processes.DefaultView;
                this.cboProcess.DisplayMember = processes.NameColumn.ToString();
                this.cboProcess.ValueMember = processes.ProcessIDColumn.ToString();

                int processID = Settings.Default.LastReportProcessID;

                if(processID > 0)
                {
                    var item = this.cboProcess.Items.ValueList.FindByDataValue(processID);
                    if(item != null)
                        this.cboProcess.SelectedItem = item;
                }

                if(this.cboProcess.SelectedIndex < 1)
                    this.cboProcess.SelectedIndex = 0;

                this.cboOrderStatus.Items.Clear();
                this.cboOrderStatus.Items.Add(OrdersByProcessReport.OrderStatus.All);
                this.cboOrderStatus.Items.Add(OrdersByProcessReport.OrderStatus.Open);
                this.cboOrderStatus.Items.Add(OrdersByProcessReport.OrderStatus.Completed);
                this.cboOrderStatus.SelectedIndex = 0;

                cboProcess.EndUpdate();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error loading processes.");
            }
        }

        private async Task RunReport()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                StartRunningReport();

                if (this.cboProcess.SelectedItem == null)
                {
                    // Invalid options
                    return;
                }

                var report = new OrdersByProcessReport();
                report.ProcessID = Convert.ToInt32(this.cboProcess.SelectedItem.DataValue);

                var processAliases = new List <int>();
                cboProcessAlias.CheckedItems.ForEach(item => processAliases.Add(Convert.ToInt32(item.DataValue)));
                report.ProcessAliases = processAliases;

                report.Status = (OrdersByProcessReport.OrderStatus) this.cboOrderStatus.SelectedItem.DataValue;


                Settings.Default.LastReportProcessID = report.ProcessID;
                Settings.Default.Save();

                await Task.Run(() => report.DisplayReport(_cancellationTokenSource.Token));
            }
            finally
            {
                StopRunningReport();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void StartRunningReport()
        {
            ControlBox = false;
            cboOrderStatus.Enabled = false;
            cboProcess.Enabled = false;
            cboProcessAlias.Enabled = false;
            chkActiveOnly.Enabled = false;
            btnOK.Enabled = false;

            this.activityIndicator.Visible = true;
            this.activityIndicator.Start();
        }

        private void StopRunningReport()
        {
            if (IsDisposed)
            {
                return;
            }

            ControlBox = true;
            cboOrderStatus.Enabled = true;
            cboProcess.Enabled = true;
            cboProcessAlias.Enabled = true;
            chkActiveOnly.Enabled = true;
            btnOK.Enabled = true;

            this.activityIndicator.Visible = false;
            this.activityIndicator.Stop();
        }

        private void UpdateAliasList()
        {
            this.cboProcessAlias.Items.Clear();

            cboProcessAlias.Enabled = cboProcess.SelectedItem != null;

            if (cboProcess.SelectedItem != null)
            {
                var processId = Convert.ToInt32(this.cboProcess.SelectedItem.DataValue);

                using (var taProcessesAlias = new ProcessAliasSummaryTableAdapter())
                {
                    var processAliases = taProcessesAlias.GetDataBy(processId);

                    foreach (var pa in processAliases)
                        cboProcessAlias.Items.Add(new Infragistics.Win.ValueListItem(pa.ProcessAliasID, pa.Name) { CheckState = CheckState.Checked });
                }

                cboProcessAlias.SelectedIndex = 0;
            }
        }

        #endregion

        #region Events

        private void OrderProcessOptions_Load(object sender, EventArgs e) { LoadProcesses(); }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                await RunReport();
                Close();
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error running report", exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc,
                    "Error canceling dialog.");
            }
        }

        private void chkActiveOnly_CheckedChanged(object sender, EventArgs e) { LoadProcesses(); }

        private void cboProcess_SelectionChanged(object sender, EventArgs e) { UpdateAliasList(); }

        #endregion
    }
}
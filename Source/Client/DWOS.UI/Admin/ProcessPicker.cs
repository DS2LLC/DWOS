using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ProcessesDatasetTableAdapters;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class ProcessPicker : Form
    {
        #region Fields

        private bool _includePlanned;
        private int? _customerID;
        private bool _initialDataLoaded;
        private bool _isLoading;
        private DataTable _processSummaryTable;

        #endregion

       
        #region Methods

        /// <summary>
        /// Constructor; will not include planned processes.
        /// </summary>
        public ProcessPicker()
            : this(false)
        {
        }

        /// <summary>
        /// Constructor for an instance that cbnditionally includes
        /// planned processes.
        /// </summary>
        /// <param name="includePlanned">
        /// true if planned processes should be included; otherwise, false
        /// </param>
        public ProcessPicker(bool includePlanned)
        {
            _includePlanned = includePlanned;
            InitializeComponent();
        }

        public void LoadExistingDataset(ProcessesDataset process)
        {
            dsProcesses = process;
            
            _initialDataLoaded = true;
        }

        public List <SelectedProcess> SelectedProcesses
        {
            get
            {
                var processes = new List <SelectedProcess>();

                foreach(UltraGridRow selectedRow in this.grdProcesses.Selected.Rows)
                {
                    var sp = new SelectedProcess {AliasName = GetSelectedRowCellValueString(selectedRow, "Alias Name"), Department = GetSelectedRowCellValueString(selectedRow, "Department"), ProcessID = GetSelectedRowCellValueInt(selectedRow, "ProcessID"), ProcessAliasID = GetSelectedRowCellValueInt(selectedRow, "ProcessAliasID")};

                    processes.Add(sp);
                }

                return processes;
            }
        }

        private int GetSelectedRowCellValueInt(UltraGridRow row, string cellName)
        {
            try
            {
                if(row != null && row.IsDataRow && row.Cells.Exists(cellName))
                {
                    object o = row.Cells[cellName].Value;

                    if(o != null && o != DBNull.Value)
                        return Convert.ToInt32(o);
                }

                return -1;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error converting getting cell value for: " + cellName;
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                return -1;
            }
        }

        private string GetSelectedRowCellValueString(UltraGridRow row, string cellName)
        {
            try
            {
                if(row != null && row.IsDataRow && row.Cells.Exists(cellName))
                {
                    object o = row.Cells[cellName].Value;

                    if(o != null && o != DBNull.Value)
                        return Convert.ToString(o);
                }

                return null;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error converting getting cell value for: " + cellName;
                LogManager.GetCurrentClassLogger().Error(exc, errorMsg);
                return null;
            }
        }

        public void LoadCustomerAliases(int? customerID)
        {
            this._isLoading = true;

            LoadData();
            
            if(customerID.HasValue && (!this._customerID.HasValue || customerID != this._customerID)) //load if changes from current
            {
                this._customerID = customerID;
                
                var customerProcesses = new ProcessesDataset.CustomerProcessAliasDataTable();
                this.taCustomerProcess.FillByCustomerID(customerProcesses, customerID.Value, true);
                
                using(var taCustomer = new CustomerTableAdapter())
                {
                    var customers = taCustomer.GetDataBy(this._customerID.Value);
                    this.lblCustomer.Text = customers != null && customers.Count > 0 ? customers.First().Name : "";
                }

                pnlCustomerInfo.Visible = true;

                //reset all preferred states to default
                foreach(DataRow processRow in _processSummaryTable.Rows)
                    processRow["Preferred"] = false;

                //for each customer alias update process summary row with the customer alias name
                foreach (var customerAlias in customerProcesses)
                {
                    var summaryRows = _processSummaryTable.Select("ProcessAliasID = " + customerAlias.ProcessAliasID);
                    foreach (var summaryRow in summaryRows)
                    {
                        summaryRow["Alias Name"] = customerAlias.Name;
                        summaryRow["Preferred"] = true;
                    }
                }

                this.lblTotalCount.Text    = this._processSummaryTable.Rows.Count.ToString();
                this.lblCustomerCount.Text = customerProcesses.Rows.Count.ToString();

                //force the refresh of the coloring of the rows, wont always fire when we change 'Preferred' after loading the second time
                grdProcesses.Rows.Refresh(RefreshRow.FireInitializeRow);
            }

            this._isLoading = false;

            var customProcessCount = _processSummaryTable.Select("Preferred = 1").Length;
            chkPreferred.Checked = customProcessCount > 0;
        }

        private void ToggleFilterByPreffered(bool filter)
        {
            //toggle the preferred only on or off
            this.grdProcesses.DisplayLayout.Bands[0].ColumnFilters["Preferred"].ClearFilterConditions();

            //only filter if set to true, else show either true or false
            if(filter)
                this.grdProcesses.DisplayLayout.Bands[0].ColumnFilters["Preferred"].FilterConditions.Add(FilterComparisionOperator.Equals, filter);
        }

        private void LoadData()
        {
            if(this._initialDataLoaded)
                return;

            this.dsProcesses.EnforceConstraints = false;

            if (this._includePlanned)
            {
                this.taProcess.FillByActive(this.dsProcesses.Process, true);
                this.taProcessAlias.FillByActive(this.dsProcesses.ProcessAlias, true);
            }
            else
            {
                this.taProcess.FillBy(this.dsProcesses.Process, true, true);
                this.taProcessAlias.FillBy(this.dsProcesses.ProcessAlias, true, true);
            }

            this._processSummaryTable = new DataTable();
            this._processSummaryTable.Columns.Add("ProcessID", typeof(int));
            this._processSummaryTable.Columns.Add("ProcessAliasID", typeof(int));
            this._processSummaryTable.Columns.Add("Alias Name", typeof(string));
            this._processSummaryTable.Columns.Add("Process Name", typeof(string));
            this._processSummaryTable.Columns.Add("Preferred", typeof(bool));
            this._processSummaryTable.Columns.Add("Department", typeof(string));

            foreach (var pa in this.dsProcesses.ProcessAlias)
                this._processSummaryTable.Rows.Add(pa.ProcessID, pa.ProcessAliasID, pa.Name, pa.ProcessRow.Name, false, pa.ProcessRow.Department);

            this.grdProcesses.DataSource = this._processSummaryTable;

            this._initialDataLoaded = true;
        }

        private void OnDispose()
        {
            this.grdProcesses.DataSource = null;

            if(this._processSummaryTable != null)
            {
                this._processSummaryTable.Dispose();
                this._processSummaryTable = null;
            }
        }

        #endregion

        #region Events

        private void ProcessPicker_Load(object sender, EventArgs e) { LoadData(); }

        private void chkPreferred_CheckedChanged(object sender, EventArgs e)
        {
            if(this._isLoading)
                return;

            ToggleFilterByPreffered(this.chkPreferred.Checked);
        }

        private void grdProcesses_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            this.grdProcesses.DisplayLayout.Bands[0].Columns["ProcessID"].Hidden = true;
            this.grdProcesses.DisplayLayout.Bands[0].Columns["ProcessAliasID"].Hidden = true;
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Preferred"].Hidden = true;
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Alias Name"].Header.Caption = "Process Alias";
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Alias Name"].Header.ToolTipText = "The processes alias or customer alias name.";
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Process Name"].Header.Caption = "Process Code";
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Process Name"].Header.ToolTipText = "The name of the process.";
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Department"].Header.ToolTipText = "The department the process belongs to.";

            //sort by Alias
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Alias Name"].SortIndicator = SortIndicator.Ascending;

            //move columns
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Department"].Header.SetVisiblePosition(0, true);
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Alias Name"].Header.SetVisiblePosition(1, true);
            this.grdProcesses.DisplayLayout.Bands[0].Columns["Process Name"].Header.SetVisiblePosition(2, true);

            //Add to group by
            this.grdProcesses.DisplayLayout.Bands[0].SortedColumns.Clear();
            this.grdProcesses.DisplayLayout.Bands[0].SortedColumns.Add("Department", false, true);
        }

        private void grdProcesses_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //if is customer pref
            if((bool) e.Row.Cells["Preferred"].Value)
            {
                e.Row.CellAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
                e.Row.CellAppearance.AlphaLevel = 200;
                e.Row.CellAppearance.BackColor = Color.LightGreen;
            }
            else
                e.Row.CellAppearance.Reset();
        }

        private void grdProcesses_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (this.grdProcesses.Selected.Rows.Count > 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        #endregion

       
    }
}
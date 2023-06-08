using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProcessSearchField = DWOS.Data.Datasets.ProcessesDatasetTableAdapters.ProcessSearchTableAdapter.ProcessSearchField;

namespace DWOS.UI.Admin
{
    /// <summary>
    /// Advanced Process Search dialog box.
    /// </summary>
    public partial class ProcessSearch : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Set of IDs of processes whose aliases and customer
        /// aliases are loaded.
        /// </summary>
        private HashSet<int> _loadedProcesses = new HashSet<int>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently selected Process.
        /// </summary>
        public DWOS.Data.Datasets.ProcessesDataset.ProcessSearchRow SelectedProcess
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the currently selected Process Alias.
        /// </summary>
        public DWOS.Data.Datasets.ProcessesDataset.ProcessAliasSearchRow SelectedAlias
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the currently selected Customer Process Alias.
        /// </summary>
        public DWOS.Data.Datasets.ProcessesDataset.CustomerProcessAliasSearchRow SelectedCustomerAlias
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the currently selected field to use for searches.
        /// </summary>
        private ProcessSearchField SelectedField
        {
            get
            {
                return (ProcessSearchField)cboProcessSearchField.SelectedItem.DataValue;
            }
        }

        /// <summary>
        ///  Gets text to use for searches.
        /// </summary>
        private string SearchText
        {
            get
            {
                return txtSearch.Text;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of <see cref="ProcessSearch"/>.
        /// </summary>
        public ProcessSearch()
        {
            InitializeComponent();
        }

        private void Search()
        {
            try
            {
                using (new Utilities.UsingWaitCursor(this))
                {
                    taProcessSearch.FillBySearch(dsProcesses.ProcessSearch, SelectedField, SearchText, chkActiveOnly.Checked);
                    grdProcesses.PerformAction(Infragistics.Win.UltraWinGrid.UltraGridAction.ToggleRowSel);
                    lblRecordCount.Text = string.Format("Record Count: {0}", grdProcesses.Rows.GetFilteredInNonGroupByRows().Length);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error searching for processes; Field:{0}, Search:{1}, Active:{2}".FormatWith(SelectedField, SearchText, chkActiveOnly.Checked));
            }
        }

        private void UpdateSelectedProcess()
        {
            bool hasSelection = grdProcesses.Selected.Rows.Count > 0 &&
                grdProcesses.Selected.Rows[0] != null &&
                grdProcesses.Selected.Rows[0].IsDataRow;

            if (hasSelection)
            {
                var rowView = (DataRowView) grdProcesses.Selected.Rows[0].ListObject;
                SelectedProcess = rowView.Row as DWOS.Data.Datasets.ProcessesDataset.ProcessSearchRow;
                SelectedAlias = rowView.Row as DWOS.Data.Datasets.ProcessesDataset.ProcessAliasSearchRow;
                SelectedCustomerAlias = rowView.Row as DWOS.Data.Datasets.ProcessesDataset.CustomerProcessAliasSearchRow;
            }
            else
            {
                SelectedProcess = null;
            }
        }

        #endregion

        #region Events

        private void ProcessSearch_Load(object sender, EventArgs e)
        {
            var listItems = new Infragistics.Win.ValueListItem[]
            {
                new Infragistics.Win.ValueListItem(ProcessSearchField.All, "-All-"),
                new Infragistics.Win.ValueListItem(ProcessSearchField.Alias, "Alias Name"),
                new Infragistics.Win.ValueListItem(ProcessSearchField.Category, "Category"),
                new Infragistics.Win.ValueListItem(ProcessSearchField.Name, "Code"),
                new Infragistics.Win.ValueListItem(ProcessSearchField.CustomerAlias, "Customer Alias Name"),
                new Infragistics.Win.ValueListItem(ProcessSearchField.CustomerName, "Customer Name"),
                new Infragistics.Win.ValueListItem(ProcessSearchField.Department, "Department"),
            };

            cboProcessSearchField.Items.AddRange(listItems);
            cboProcessSearchField.SelectedIndex = 0;
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Search();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void grdProcesses_BeforeRowExpanded(object sender, Infragistics.Win.UltraWinGrid.CancelableRowEventArgs e)
        {
            try
            {
                var processID = Convert.ToInt32(e.Row.GetCellValue("ProcessID"));

                if (processID > 0 && !_loadedProcesses.Contains(processID))
                {
                    taProcessAliasSearch.FillByProcessID(dsProcesses.ProcessAliasSearch, processID);
                    taCustomerProcessAliasSearch.FillByProcessID(dsProcesses.CustomerProcessAliasSearch, processID);
                    _loadedProcesses.Add(processID);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading aliases for a process search.");
            }
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSelectedProcess();

                if (SelectedProcess != null || SelectedAlias != null || SelectedCustomerAlias != null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error in btnGoTo click handler.");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSelectedProcess();
                // Because of btnOK's properties, the dialog is closed (w/ result 'OK')
                // even if there is not a selected process.
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error in btnOK click handler.");
            }
        }

        #endregion
    }
}

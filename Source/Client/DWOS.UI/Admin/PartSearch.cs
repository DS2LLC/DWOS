using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class PartSearch : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public Data.Datasets.PartsDataset.PartSearchRow SelectedPart { get; set; }

        #endregion

        #region Methods
        
        public PartSearch()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            cboQuoteSearchField.Items.Add(Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField.All, "-All-");
            cboQuoteSearchField.Items.Add(Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField.Airframe, "Model");
            cboQuoteSearchField.Items.Add(Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField.AssemblyNumber, "Assembly Number");
            cboQuoteSearchField.Items.Add(Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField.CustomerName, "Customer Name");
            cboQuoteSearchField.Items.Add(Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField.Material, "Material");
            cboQuoteSearchField.Items.Add(Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField.PartName, "Part Name");
            cboQuoteSearchField.SelectedIndex = 0;
        }

        private void Search(Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField searchField, string search)
        {
            try
            {
                using(new Utilities.UsingWaitCursor(this))
                {
                    taPartSearch.FillBySearch(dsParts.PartSearch, searchField, search, chkActiveOnly.Checked);

                    grdParts.PerformAction(UltraGridAction.ToggleRowSel);
                    lblRecordCount.Text = "Record Count: " + grdParts.Rows.GetFilteredInNonGroupByRows().Length;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error searching for parts.");
            }
        }

        #endregion

        #region Events

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search((Data.Datasets.PartsDatasetTableAdapters.PartSearchTableAdapter.PartSearchField)cboQuoteSearchField.SelectedItem.DataValue, txtSearch.Text);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdParts.Selected.Rows.Count > 0)
                {
                    var row = grdParts.Selected.Rows[0];

                    if (row != null && row.IsDataRow && row.Band.Columns.Exists("PartID"))
                    {
                        var partID = Convert.ToInt32(row.GetCellValue("PartID"));

                        using (var p = new QuickViewPart())
                        {
                            p.PartID = partID;
                            p.ShowDialog(this);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error showing part quick view.");
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
                btnSearch.PerformClick();
        }

        private void PartSearch_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdParts.Selected.Rows.Count > 0)
                {
                    var row = grdParts.Selected.Rows[0];
                    if (row != null && row.IsDataRow && row.Band.Columns.Exists("PartID"))
                    {
                        this.SelectedPart = ((DataRowView)row.ListObject).Row as Data.Datasets.PartsDataset.PartSearchRow;
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error in go to part.");
            }
        }

        #endregion
    }
}

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
    public partial class QuoteSearch : Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public Data.Datasets.QuoteDataSet.QuoteSearchRow SelectedQuote { get; private set; }

        public Data.Datasets.QuoteDataSet.QuotePartSearchRow SelectedQuotePart { get; private set; }
        
        #endregion

        #region Methods

        public QuoteSearch()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            cboQuoteSearchField.Items.Add(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField.All, "-All-");
            cboQuoteSearchField.Items.Add(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField.ContactName, "Contact Name");
            cboQuoteSearchField.Items.Add(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField.CustomerName, "Customer Name");
            cboQuoteSearchField.Items.Add(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField.Program, "Program");
            cboQuoteSearchField.Items.Add(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField.QuoteID, "Quote Id");
            cboQuoteSearchField.Items.Add(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField.RFQ, "RFQ");
            cboQuoteSearchField.Items.Add(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField.PartName, "Part Name");
            cboQuoteSearchField.SelectedIndex = 0;
        }

        private void Search(Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField searchField, string search)
        {
            try
            {
                using(new Utilities.UsingWaitCursor(this))
                {
                    //clear parts table to prevent constraint errors
                    dsQuotes.QuotePartSearch.Clear();

                    taQuoteSearch.FillBySearch(dsQuotes.QuoteSearch, searchField, search, chkActiveOnly.Checked);

                    grdParts.PerformAction(UltraGridAction.ToggleRowSel);
                    lblRecordCount.Text = "Record Count: " + grdParts.Rows.GetFilteredInNonGroupByRows().Length;
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error searching for quotes; Field:{0}, Search:{1}, Active:{2}".FormatWith(searchField, search, chkActiveOnly.Checked));
            }
        }

        private bool UpdateSelectedRow()
        {
            if (grdParts.Selected.Rows.Count > 0)
            {
                var row = grdParts.Selected.Rows[0];

                if (row != null && row.IsDataRow)
                {
                    this.SelectedQuote = ((DataRowView)row.ListObject).Row as Data.Datasets.QuoteDataSet.QuoteSearchRow;
                    this.SelectedQuotePart = ((DataRowView)row.ListObject).Row as Data.Datasets.QuoteDataSet.QuotePartSearchRow;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Events

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search((Data.Datasets.QuoteDataSetTableAdapters.QuoteSearchTableAdapter.QuoteSearchField)cboQuoteSearchField.SelectedItem.DataValue, txtSearch.Text);
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            try
            {
                if (UpdateSelectedRow())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error in go to part.");
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
                btnSearch.PerformClick();
        }

        private void QuoteSearch_Load(object sender, EventArgs e) { LoadData(); }

        private void grdParts_BeforeRowExpanded(object sender, CancelableRowEventArgs e)
        {
            try
            {
                var quoteID = Convert.ToInt32(e.Row.GetCellValue("QuoteID"));

                if (quoteID > 0)
                    taQuotePartSearch.FillByQuoteID(dsQuotes.QuotePartSearch, quoteID);
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error loading parts for a quote search.");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (UpdateSelectedRow())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
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

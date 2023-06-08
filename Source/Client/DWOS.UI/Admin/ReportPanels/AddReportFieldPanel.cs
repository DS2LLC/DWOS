using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.CustomersDatasetTableAdapters;
using DWOS.Data.Datasets.ReportFieldsDataSetTableAdapters;
using Infragistics.Documents.Excel.Filtering;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Windows.DataPresenter;

namespace DWOS.UI.Admin.ReportPanels
{
    public partial class AddReportFieldPanel : Form
    {
        #region Properties

        public string SelectedField { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="AddReportFieldPanel" /> class.
        /// </summary>
        /// <param name="cboList">The cbo list.</param>
        public AddReportFieldPanel(List<string> cboList)
        {
            InitializeComponent();
            cboField.DataSource = cboList;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
       
        #endregion Methods

        #region Events

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedField = cboField.SelectedItem.DisplayText;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cboField_Changed(object sender, EventArgs e)
        {
            btnOK.Enabled = cboField.SelectedItem != null;
        }

        #endregion Events
    }
}

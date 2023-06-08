using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales
{
    public partial class OrderReviewStatus : Form
    {
        #region Properties

        public string Notes
        {
            get { return txtNotes.Text;  }
        }

        public bool ReviewStatus
        {
            get { return cboStatus.SelectedItem != null && Convert.ToBoolean(cboStatus.SelectedItem.DataValue); }
        }

        #endregion

        #region Methods
        
        public OrderReviewStatus()
        {
            InitializeComponent();
        }

        public void LoadData(int userID, int orderID, OrdersDataSet dataset)
        {
            txtWorkOrder.Text           = orderID.ToString();

            cboReviewUser.DataSource    = dataset.UserSummary;
            cboReviewUser.DisplayMember = dataset.UserSummary.NameColumn.ColumnName;
            cboReviewUser.ValueMember   = dataset.UserSummary.UserIDColumn.ColumnName;

            var item = cboReviewUser.FindItemByValue<int>(p => p == userID);
            if (item != null)
                cboReviewUser.SelectedItem = item;
        }

        #endregion
    }
}

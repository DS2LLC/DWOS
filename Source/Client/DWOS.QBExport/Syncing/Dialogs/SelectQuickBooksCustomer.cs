using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using Infragistics.Win;
using nsoftware.InQB;
using NLog;

namespace DWOS.QBExport.Syncing.Dialogs
{
    public partial class SelectQuickBooksCustomer : Form
    {
        #region Properties

        public string SelectedCustomerName =>
            chkNew.Checked ? null : cboQuickBooksCustomer.Text;

        #endregion

        #region Methods

        public SelectQuickBooksCustomer()
        {
            InitializeComponent();
        }

        public void LoadData(CustomersDataset.CustomerRow customerRow, IEnumerable<Customer> quickBooksCustomers)
        {
            if (customerRow == null)
            {
                throw new ArgumentNullException(nameof(customerRow));
            }

            txtCustomer.Text = customerRow.Name;

            cboQuickBooksCustomer.Items.Clear();
            var orderedItems = quickBooksCustomers.OrderBy(c => c.CustomerName).Select(c => new ValueListItem(c.CustomerName)).ToArray();
            cboQuickBooksCustomer.Items.AddRange(orderedItems);

            var quickBooksHasCustomers = orderedItems.Length > 0;

            if (quickBooksHasCustomers)
            {
                cboQuickBooksCustomer.SelectedIndex = 0;
            }

            cboQuickBooksCustomer.Enabled = quickBooksHasCustomers;
            chkNew.Enabled = quickBooksHasCustomers;
            chkExisting.Enabled = quickBooksHasCustomers;

            chkNew.Checked = true;
        }

        #endregion

        #region Events

        private void chkExisting_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var isExistingChecked = chkExisting.Checked;
                chkNew.Checked = !isExistingChecked;

                cboQuickBooksCustomer.Enabled = isExistingChecked;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing 'existing customer' checkbox.");
            }
        }

        private void chkNew_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var isNewChecked = chkNew.Checked;
                chkExisting.Checked = !isNewChecked;
                cboQuickBooksCustomer.Enabled = !isNewChecked;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing 'new customer' checkbox.");
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using Infragistics.Win;
using NLog;
using nsoftware.InQB;

namespace DWOS.QBExport.Syncing.Dialogs
{
    public partial class SelectDwosCustomer : Form
    {
        #region Properties

        public CustomersDataset.CustomerRow SelectedCustomer =>
            chkNew.Checked ? null : cboDwosCustomer.Value as CustomersDataset.CustomerRow;

        #endregion

        #region Methods

        public SelectDwosCustomer()
        {
            InitializeComponent();
        }

        public void LoadData(Customer quickBooksCustomer, IEnumerable<CustomersDataset.CustomerRow> existingCustomers)
        {
            if (quickBooksCustomer == null)
            {
                throw new ArgumentNullException(nameof(quickBooksCustomer));
            }

            if (existingCustomers == null)
            {
                throw new ArgumentNullException(nameof(existingCustomers));
            }

            txtCustomer.Text = string.IsNullOrEmpty(quickBooksCustomer.CompanyName)
                ? quickBooksCustomer.CustomerName
                : quickBooksCustomer.CompanyName;

            cboDwosCustomer.Items.Clear();
            var orderedItems = existingCustomers
                .Where(c => c.Active)
                .OrderBy(c => c.Name)
                .Select(c => new ValueListItem(c, c.Name))
                .ToArray();

            cboDwosCustomer.Items.AddRange(orderedItems);
            var dwosHasCustomers = orderedItems.Length > 0;

            if (dwosHasCustomers)
            {
                cboDwosCustomer.SelectedIndex = 0;
            }

            cboDwosCustomer.Enabled = dwosHasCustomers;
            chkNew.Enabled = dwosHasCustomers;
            chkExisting.Enabled = dwosHasCustomers;

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

                cboDwosCustomer.Enabled = isExistingChecked;
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
                cboDwosCustomer.Enabled = !isNewChecked;
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing 'new customer' checkbox.");
            }
        }

        #endregion
    }
}

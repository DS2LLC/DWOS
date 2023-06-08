using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Customer
{
    public partial class AddRelationshipDialog : Form
    {
        #region Properties

        public CustomersDataset.CustomerRow SelectedCustomer =>
            cboRelatedCustomer.SelectedItem?.ListObject as CustomersDataset.CustomerRow;

        #endregion

        #region Methods

        public AddRelationshipDialog()
        {
            InitializeComponent();
        }

        public void LoadData(CustomersDataset.CustomerRow customerA, IEnumerable<CustomersDataset.CustomerRow> customers)
        {
            if (customerA == null)
            {
                throw new ArgumentNullException(nameof(customerA));
            }

            if (customers == null)
            {
                throw new ArgumentNullException(nameof(customers));
            }

            txtCurrentCustomer.Text = customerA.Name;
            cboRelatedCustomer.DataSource = customers.ToList();
            cboRelatedCustomer.DisplayMember = nameof(CustomersDataset.CustomerRow.Name);
            cboRelatedCustomer.ValueMember = nameof(CustomersDataset.CustomerRow.CustomerID);

            if (cboRelatedCustomer.Items.Count > 0)
            {
                cboRelatedCustomer.SelectedIndex = 0;
            }
        }

        #endregion

        #region Events

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}

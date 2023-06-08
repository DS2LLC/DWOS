using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;
using DWOS.Data;

namespace DWOS.UI.Sales.Customer
{
    public partial class CustomerShipping : DataPanel
    {
        #region Properties

        public CustomersDataset Dataset
        {
            get { return base._dataset as CustomersDataset; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.CustomerShipping.CustomerShippingIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public CustomerShipping() { InitializeComponent(); }

        public void LoadData(CustomersDataset dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.CustomerShipping.TableName;

            //bind column to control
            base.BindValue(this.cboCarrier, Dataset.CustomerShipping.CarrierIDColumn.ColumnName);
            base.BindValue(this.txtCarrierNumber, Dataset.CustomerShipping.CarrierCustomerNumberColumn.ColumnName);
            base.BindValue(this.chkDefault, Dataset.CustomerShipping.DefaultShippingMethodColumn.ColumnName);
            BindValue(chkActive, Dataset.CustomerShipping.ActiveColumn.ColumnName);

            this.chkDefault.CheckedChanged += chkDefault_CheckedChanged;

            //bind lists
            base.BindList(this.cboCarrier, Dataset.d_ShippingCarrier, Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName, Dataset.d_ShippingCarrier.CarrierIDColumn.ColumnName);

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            var codeField = FieldUtilities.GetField("Customer", "Carrier Code");

            if (codeField.IsVisible)
            {
                manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtCarrierNumber, "Shipping carrier customer number required.")
                {
                    DefaultValue = "XXXXXX",
                    IsRequired = codeField.IsRequired,
                    ValidationRequired = () => IsNewRow ?? false
                }, errProvider));
            }
            else
            {
                lblCarrierNumber.Visible = false;
                txtCarrierNumber.Visible = false;
            }

            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.cboCarrier, "Carrier selection required."), errProvider));
        }

        public CustomersDataset.CustomerShippingRow AddShippingRow(int userID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as CustomersDataset.CustomerShippingRow;
            cr.CustomerID = userID;
            cr.CarrierID = Dataset.d_ShippingCarrier[0].CarrierID;
            cr.CarrierCustomerNumber = "XXXXXX";
            cr.DefaultShippingMethod = true;

            return cr;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Handles the CheckedChanged event of the chkDefault control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void chkDefault_CheckedChanged(object sender, EventArgs e)
        {
            if(this.chkDefault.Checked)
            {
                var customerID = ((CustomersDataset.CustomerShippingRow) Dataset.CustomerShipping.Rows[0]).CustomerID;
                var results = Dataset.CustomerShipping.Where(cs => cs.CustomerID == customerID && cs.CarrierID != this.cboCarrier.Value.ToString());

                // If this shipping option is marked as default we need to uncheck any other options marked as default
                foreach(var result in results)
                    result.DefaultShippingMethod = false;

                // Refresh the parent so the correct node is bold
                if(ParentForm != null)
                    ((Customers) ParentForm).RefreshSelectedNodeParent();
            }
        }

        #endregion Events
    }
}
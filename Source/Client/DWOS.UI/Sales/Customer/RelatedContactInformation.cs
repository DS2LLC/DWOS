using System;
using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Customer
{
    public partial class RelatedContactInformation : DataPanel
    {
        #region Fields

        public event EventHandler<GoToContactEventArgs> GoToContactClicked;
        public event EventHandler<GoToCustomerEventArgs> GoToCustomerClicked;

        #endregion

        #region Properties

        public CustomersDataset Dataset
        {
            get => _dataset as CustomersDataset;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.Contact.ContactIDColumn.ColumnName;

        #endregion

        #region Methods

        public RelatedContactInformation()
        {
            InitializeComponent();
        }

        public void LoadData(CustomersDataset dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.Contact.TableName;

            BindValue(txtContactName, Dataset.Contact.NameColumn.ColumnName);
            _panelLoaded = true;
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            base.BeforeMoveToNewRecord(id);
            txtCustomerName.Clear();
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var contact = CurrentRecord as CustomersDataset.ContactRow;

            if (contact == null)
            {
                return;
            }

            txtCustomerName.Text = contact.CustomerRow?.Name;
        }

        #endregion

        #region Events

        private void btnGoToOriginal_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(CurrentRecord is CustomersDataset.ContactRow contact))
                {
                    return;
                }

                GoToContactClicked?.Invoke(this, new GoToContactEventArgs(contact.ContactID));
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error navigating to original contact.");
            }
        }

        private void btnGoToCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(CurrentRecord is CustomersDataset.ContactRow contact))
                {
                    return;
                }

                GoToCustomerClicked?.Invoke(this, new GoToCustomerEventArgs(contact.CustomerID));
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error navigating to original customer.");
            }
        }

        #endregion

        #region GoToContactEventArgs

        public class GoToContactEventArgs : EventArgs
        {
            #region Properties

            public int ContactId { get; }

            #endregion

            #region Methods

            public GoToContactEventArgs(int contactId)
            {
                ContactId = contactId;
            }

            #endregion
        }

        #endregion

        #region GoToCustomerEventArgs

        public class GoToCustomerEventArgs : EventArgs
        {
            #region Properties

            public int CustomerId { get; }

            #endregion

            #region Methods

            public GoToCustomerEventArgs(int customerId)
            {
                CustomerId = customerId;
            }

            #endregion
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.Shared;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;
using Infragistics.Win.UltraWinEditors;

namespace DWOS.UI.Sales
{
    public partial class CustomerContactInformation : DataPanel
    {
        #region Fields

        private ContactSummaryTableAdapter taContactSummary;

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get => _dataset as OrdersDataSet;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.CustomerCommunication.CusomterCommunicationIDColumn.ColumnName;

        public List<int> ValidCustomerIds { get; set; } = new List<int>();

        #endregion

        #region Methods

        public CustomerContactInformation() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset, ContactSummaryTableAdapter contactSummary)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.CustomerCommunication.TableName;

            taContactSummary = contactSummary;

            BindValue(dtContactDate, Dataset.CustomerCommunication.CreationDateColumn.ColumnName);
            BindValue(cboCommUserCreated, Dataset.CustomerCommunication.UserIDColumn.ColumnName);
            BindValue(cboCommContact, Dataset.CustomerCommunication.ContactIDColumn.ColumnName);
            BindValue(txtNotes, Dataset.CustomerCommunication.NotesColumn.ColumnName);

            BindList(cboCommContact, Dataset.ContactSummary, Dataset.ContactSummary.ContactIDColumn.ColumnName, Dataset.ContactSummary.NameColumn.ColumnName);
            BindList(cboCommUserCreated, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            _panelLoaded = true;

            if (!SecurityManager.Current.IsInRole("Customers.Edit") && !SecurityManager.Current.IsInRole("AddContact"))
            {
                cboCommContact.ButtonsRight[0].Enabled = false;
            }
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(cboCommContact, "Customer contact required."), errProvider));
            manager.Add(new ImageDisplayValidator(
                new TextControlValidator(cboCommUserCreated, "User required.")
                {
                    ValidationRequired = () => IsNewRow ?? true
                }, errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(dtContactDate, "Contact date required."), errProvider));
        }

        public OrdersDataSet.CustomerCommunicationRow AddCustomerCommunicationRow(int orderID, int userID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.CustomerCommunicationRow;
            cr.CreationDate = DateTime.Now;
            cr.UserID = userID;
            cr.OrderID = orderID;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            // Filter on valid customers IDs and current contact
            // Not matching on current contact may leave field blank if someone removes a customer relationship.
            var currentCommRow = CurrentRecord as OrdersDataSet.CustomerCommunicationRow;

            var contactId = -1;

            if (currentCommRow != null && !currentCommRow.IsContactIDNull())
            {
                contactId = currentCommRow.ContactID;
            }

            UpdateFilter(cboCommContact, $"CustomerID IN ({string.Join(",", ValidCustomerIds)}) OR ContactID = {contactId}");
            base.AfterMovedToNewRecord(id);
        }

        #endregion

        #region Events

        private void cboCommContact_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            try
            {
                //clicked on add new customer contact
                if (CurrentRecord != null)
                {
                    using (var p = new Customers())
                    {
                        p.CustomerFilterIds = ValidCustomerIds;

                        if (p.ShowDialog(this) == DialogResult.OK)
                        {
                            //reload list of contacts
                            taContactSummary.Fill(Dataset.ContactSummary);

                            //Refresh binding to show new customer contact added
                            cboCommContact.DataBind();

                            //if selected part in dialog then select part on order
                            if (p.SelectedContactID >= 0)
                            {
                                ((OrdersDataSet.CustomerCommunicationRow)base.CurrentRecord).ContactID = p.SelectedContactID;
                                cboCommContact.DataBindings[0].ReadValue();
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error adding contact.", exc);
            }
        }

        #endregion
    }
}
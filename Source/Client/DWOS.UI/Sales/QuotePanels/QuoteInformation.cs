using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Reports;
using DWOS.UI.Utilities;
using DWOS.Shared;

namespace DWOS.UI.Sales
{
    public partial class QuoteInformation: DataPanel
    {
        #region Fields

        private readonly List<int> _customerContactsLoaded = new List<int>();
        private readonly ConcurrentDictionary<int, List<int>> _relatedCustomers =
            new ConcurrentDictionary<int, List<int>>();

        #endregion

        #region Properties

        public QuoteDataSet Dataset
        {
            get { return base._dataset as QuoteDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return this.Dataset.Quote.QuoteIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public QuoteInformation()
        {
            this.InitializeComponent();
        }

        public void LoadData(QuoteDataSet dataset)
        {
            this.Dataset = dataset;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.Quote.TableName;

            //bind column to control
            base.BindValue(this.txtQuoteID, this.Dataset.Quote.QuoteIDColumn.ColumnName);
            base.BindValue(this.dtCreated, this.Dataset.Quote.CreatedDateColumn.ColumnName);
            base.BindValue(this.txtUser, this.Dataset.Quote.UserIdColumn.ColumnName);
            base.BindValue(this.txtRev, this.Dataset.Quote.RevisionColumn.ColumnName);
            base.BindValue(this.cboStatus, this.Dataset.Quote.StatusColumn.ColumnName);
            base.BindValue(this.dtClosed, this.Dataset.Quote.ClosedDateColumn.ColumnName);
            base.BindValue(this.txtProgram, this.Dataset.Quote.ProgramColumn.ColumnName);
            base.BindValue(this.txtRFQ, this.Dataset.Quote.RFQColumn.ColumnName);
            base.BindValue(this.dteExpiration, this.Dataset.Quote.ExpirationDateColumn.ColumnName);
            base.BindValue(this.cboCustomer, this.Dataset.Quote.CustomerIDColumn.ColumnName);
            base.BindValue(this.cboContact, this.Dataset.Quote.ContactIDColumn.ColumnName);
            base.BindValue(this.cboTerms, this.Dataset.Quote.TermsIDColumn.ColumnName);
            base.BindValue(this.txtNotes, this.Dataset.Quote.NotesColumn.ColumnName);

            ////bind lists
            base.BindList(this.cboCustomer, this.Dataset.Customer, this.Dataset.Customer.CustomerIDColumn.ColumnName, this.Dataset.Customer.NameColumn.ColumnName);
            base.BindList(this.cboContact, this.Dataset.d_ContactSummary, this.Dataset.d_ContactSummary.ContactIDColumn.ColumnName, this.Dataset.d_ContactSummary.NameColumn.ColumnName);
            base.BindList(this.cboTerms, this.Dataset.d_Terms, this.Dataset.d_Terms.TermsIDColumn.ColumnName, this.Dataset.d_Terms.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new DropDownControlValidator(this.cboCustomer, true), errProvider));
            manager.Add(new ImageDisplayValidator(new DropDownControlValidator(this.cboContact, true), errProvider));
            manager.Add(new ImageDisplayValidator(new DropDownControlValidator(this.cboTerms, true), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtCreated, "Date created required."), errProvider));
            manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dteExpiration, "Expiration date required."), errProvider));
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var quote = CurrentRecord as QuoteDataSet.QuoteRow;

            if(quote != null)
                this.LoadUserName(quote.UserId);
            else
                this.txtUser.Text = null;
        }

        private void LoadUserName(int userID)
        {
            var user = this.Dataset.Users.FindByUserID(userID);

            //Load the user to get there Name
            if(user == null)
            {
                new UsersTableAdapter{ClearBeforeFill = false}.FillBy(this.Dataset.Users, userID);
                user = this.Dataset.Users.FindByUserID(userID);
            }

            this.txtUser.Text = user == null ? "" : user.Name;
        }

        private void LoadContacts(int customerID)
        {
            if(!this._customerContactsLoaded.Contains(customerID))
            {
                using(var ta = new d_ContactSummaryTableAdapter{ClearBeforeFill = false})
                {
                    ta.Fill(this.Dataset.d_ContactSummary);
                }

                this._customerContactsLoaded.Add(customerID);
            }
        }

        #endregion

        #region Events

        private void cboCustomer_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboCustomer.SelectedItem != null)
                {
                    var quoteRow = CurrentRecord as QuoteDataSet.QuoteRow;
                    var contactId = quoteRow?.ContactID ?? -1;

                    int selectedCustomer = Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue);

                    this.LoadContacts(selectedCustomer);

                    base.UpdateFilter(this.cboContact, $"CustomerID = {selectedCustomer} OR ContactID = {contactId}");
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error changing selected customer.");
            }
        }

        private void cboContact_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                //clicked on add new customer contact
                if (base.CurrentRecord == null || this.cboCustomer.SelectedItem == null)
                {
                    return;
                }

                int selectedCustomerID = Convert.ToInt32(this.cboCustomer.SelectedItem.DataValue);
                var customerIds = new List<int> { selectedCustomerID };

                using (var p = new Customers())
                {
                    p.CustomerFilterIds = customerIds;

                    if (p.ShowDialog(this) == DialogResult.OK)
                    {
                        this._customerContactsLoaded.Remove(selectedCustomerID);
                        this.LoadContacts(selectedCustomerID);

                        if (p.SelectedContactID >= 0)
                        {
                            var currentRecord = base.CurrentRecord as QuoteDataSet.QuoteRow;

                            if (currentRecord != null)
                            {
                                currentRecord.ContactID = p.SelectedContactID;
                                this.cboContact.DataBindings[0].ReadValue();
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
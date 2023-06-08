using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DWOS.Utilities.Validation;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.QuoteDataSetTableAdapters;
using DWOS.Reports;
using DWOS.UI.Utilities;

namespace DWOS.UI.Inventory
{
    public partial class InventoryItemPanel: DataPanel
    {
        #region Fields


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

        public InventoryItemPanel()
        {
            this.InitializeComponent();
        }

        public void LoadData(QuoteDataSet dataset)
        {
            this.Dataset = dataset;
            bsData.DataSource = this.Dataset;
            bsData.DataMember = this.Dataset.Quote.TableName;

            //bind column to control
            //base.BindValue(this.txtQuoteID, this.Dataset.Quote.QuoteIDColumn.ColumnName);
            //base.BindValue(this.dtCreated, this.Dataset.Quote.CreatedDateColumn.ColumnName);
            //base.BindValue(this.txtUser, this.Dataset.Quote.UserIdColumn.ColumnName);
            //base.BindValue(this.txtRev, this.Dataset.Quote.RevisionColumn.ColumnName);
            //base.BindValue(this.cboStatus, this.Dataset.Quote.StatusColumn.ColumnName);
            //base.BindValue(this.dtClosed, this.Dataset.Quote.ClosedDateColumn.ColumnName);
            //base.BindValue(this.txtProgram, this.Dataset.Quote.ProgramColumn.ColumnName);
            //base.BindValue(this.txtRFQ, this.Dataset.Quote.RFQColumn.ColumnName);
            //base.BindValue(this.dteExpiration, this.Dataset.Quote.ExpirationDateColumn.ColumnName);
            //base.BindValue(this.cboCustomer, this.Dataset.Quote.CustomerIDColumn.ColumnName);
            //base.BindValue(this.cboContact, this.Dataset.Quote.ContactIDColumn.ColumnName);
            //base.BindValue(this.cboTerms, this.Dataset.Quote.TermsIDColumn.ColumnName);
            //base.BindValue(this.txtNotes, this.Dataset.Quote.NotesColumn.ColumnName);

            //////bind lists
            //base.BindList(this.cboCustomer, this.Dataset.Customer, this.Dataset.Customer.CustomerIDColumn.ColumnName, this.Dataset.Customer.NameColumn.ColumnName);
            //base.BindList(this.cboContact, this.Dataset.d_ContactSummary, this.Dataset.d_ContactSummary.ContactIDColumn.ColumnName, this.Dataset.d_ContactSummary.NameColumn.ColumnName);
            //base.BindList(this.cboTerms, this.Dataset.d_Terms, this.Dataset.d_Terms.TermsIDColumn.ColumnName, this.Dataset.d_Terms.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            //manager.Add(new ImageDisplayValidator(new DropDownControlValidator(this.cboCustomer, true), errProvider));
            //manager.Add(new ImageDisplayValidator(new DropDownControlValidator(this.cboContact, true), errProvider));
            //manager.Add(new ImageDisplayValidator(new DropDownControlValidator(this.cboTerms, true), errProvider));
            //manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dtCreated, "Date created required."), errProvider));
            //manager.Add(new ImageDisplayValidator(new DateTimeValidator(this.dteExpiration, "Expiration date required."), errProvider));
        }

        public QuoteDataSet.QuoteRow AddQuoteRow()
        {
            var rowVw         = bsData.AddNew() as DataRowView;
            var cr            = rowVw.Row as QuoteDataSet.QuoteRow;

            cr.CreatedDate    = DateTime.Now;
            cr.UserId         = SecurityManager.Current.CurrentUser.UserID;
            cr.Status         = "Open";
            cr.ExpirationDate = DateUtilities.AddBusinessDays(DateTime.Now, 30);
            cr.CustomerID     = -1;
            cr.ContactID      = -1;
            cr.TermsID        = this.Dataset.d_Terms.Count > 0 ? this.Dataset.d_Terms[0].TermsID : -1;

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

           
        }
        
        #endregion

        #region Events

       

        #endregion
    }
}
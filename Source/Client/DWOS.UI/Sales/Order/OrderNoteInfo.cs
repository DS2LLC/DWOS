using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using DWOS.Utilities.Validation;

namespace DWOS.UI.Sales
{
    public partial class OrderNoteInfo : DataPanel
    {
        #region Fields

        public enum NoteType
        {
            Internal,
            External
        }

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.OrderNote.OrderNoteIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public OrderNoteInfo() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderNote.TableName;

            base.BindValue(this.dteDateAdded, Dataset.OrderNote.TimeStampColumn.ColumnName);
            base.BindValue(this.cboUser, Dataset.OrderNote.UserIDColumn.ColumnName);
            base.BindValue(this.cboNoteType, Dataset.OrderNote.NoteTypeColumn.ColumnName);
            base.BindValue(this.txtNotes, Dataset.OrderNote.NotesColumn.ColumnName);

            base.BindList(this.cboUser, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider) { manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtNotes, "Notes required."), errProvider)); }

        public OrdersDataSet.OrderNoteRow AddNote(int orderID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.OrderNoteRow;

            cr.OrderID = orderID;
            cr.UserID = SecurityManager.Current.UserID;
            cr.TimeStamp = DateTime.Now;
            cr.NoteType = NoteType.Internal.ToString();

            return cr;
        }

        #endregion
    }
}
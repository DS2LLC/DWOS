using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Order
{
    public partial class HoldInfo : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.OrderHold.OrderHoldIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public HoldInfo()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            _dataset = dataset;
            bsData.DataSource = dataset;
            bsData.DataMember = dataset.OrderHold.TableName;
            _panelLoaded = true;

            base.BindValue(dteTimeIn, Dataset.OrderHold.TimeInColumn.ColumnName);
            base.BindValue(dteTimeOut, Dataset.OrderHold.TimeOutColumn.ColumnName);
            base.BindValue(cboReason, Dataset.OrderHold.HoldCodeColumn.ColumnName);
            base.BindValue(txtNotes, Dataset.OrderHold.NotesColumn.ColumnName);

            base.BindList(cboReason,
                Dataset.d_HoldReason,
                Dataset.d_HoldReason.HoldReasonIDColumn.ColumnName,
                Dataset.d_HoldReason.NameColumn.ColumnName);
        }

        #endregion
    }
}

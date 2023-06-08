using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Order
{
    public partial class BulkCOCInformation : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.BulkCOC.BulkCOCIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public BulkCOCInformation()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            _dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.BulkCOC.TableName;

            base.BindValue(this.dteDateCertified, Dataset.BulkCOC.DateCertifiedColumn.ColumnName);
            base.BindValue(this.cboCreatedBy, Dataset.BulkCOC.QAUserColumn.ColumnName);
            base.BindValue(this.txtBulkCOCID, Dataset.BulkCOC.BulkCOCIDColumn.ColumnName);
            base.BindValue(this.txtPackage, Dataset.BulkCOC.ShipmentPackageIDColumn.ColumnName);

            base.BindList(this.cboCreatedBy, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            _panelLoaded = true;
        }

        #endregion
    }
}

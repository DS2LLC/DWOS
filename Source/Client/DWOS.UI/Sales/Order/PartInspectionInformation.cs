using DWOS.Data.Datasets;

namespace DWOS.UI.Sales
{
    public partial class PartInspectionInformation : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get => _dataset as OrdersDataSet;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey => Dataset.PartInspection.PartInspectionIDColumn.ColumnName;

        #endregion

        #region Methods

        public PartInspectionInformation() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.PartInspection.TableName;

            BindValue(dteInspectionDate, Dataset.PartInspection.InspectionDateColumn.ColumnName);
            BindValue(cboInspectionType, Dataset.PartInspection.PartInspectionTypeIDColumn.ColumnName);
            BindValue(cboUser, Dataset.PartInspection.QAUserIDColumn.ColumnName);
            BindValue(numAccepted, Dataset.PartInspection.AcceptedQtyColumn.ColumnName);
            BindValue(numRejected, Dataset.PartInspection.RejectedQtyColumn.ColumnName);
            BindValue(txtNotes, Dataset.PartInspection.NotesColumn.ColumnName);

            BindList(cboInspectionType,
                Dataset.PartInspectionType,
                Dataset.PartInspectionType.PartInspectionTypeIDColumn.ColumnName,
                Dataset.PartInspectionType.NameColumn.ColumnName);

            BindList(cboUser,
                Dataset.UserSummary,
                Dataset.UserSummary.UserIDColumn.ColumnName,
                Dataset.UserSummary.NameColumn.ColumnName);

            _panelLoaded = true;
        }

        #endregion
    }
}

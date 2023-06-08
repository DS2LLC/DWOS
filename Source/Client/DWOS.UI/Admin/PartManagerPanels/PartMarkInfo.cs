using DWOS.Data.Datasets;

namespace DWOS.UI.Admin.PartManagerPanels
{
    public partial class PartMarkInfo : DataPanel
    {
        public PartsDataset Dataset
        {
            get => _dataset as PartsDataset;
            set => _dataset = value;
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.Part_PartMarking.Part_PartMarkingIDColumn.ColumnName;

        public PartMarkInfo()
        {
            InitializeComponent();
        }

        public void LoadData(PartsDataset dataset)
        {
            Dataset = dataset;

            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.Part_PartMarking.TableName;

            BindValue(txtLine1, Dataset.Part_PartMarking.Def1Column.ColumnName);
            BindValue(txtLine2, Dataset.Part_PartMarking.Def2Column.ColumnName);
            BindValue(txtLine3, Dataset.Part_PartMarking.Def3Column.ColumnName);
            BindValue(txtLine4, Dataset.Part_PartMarking.Def4Column.ColumnName);
            BindValue(txtSpecification, Dataset.Part_PartMarking.ProcessSpecColumn.ColumnName);

            _panelLoaded = true;
        }
    }
}

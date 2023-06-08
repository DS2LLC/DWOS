using DWOS.Data;
using DWOS.Data.Datasets;
using System.Data;
using System.Linq;

namespace DWOS.UI.Sales.Order
{
    public partial class BillOfLadingInformation : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.BillOfLading.BillOfLadingIDColumn.ColumnName;

        #endregion

        #region Methods

        public BillOfLadingInformation()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dataset)
        {
            _dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.BillOfLading.TableName;

            BindValue(dteDate, Dataset.BillOfLading.DateCreatedColumn.ColumnName);
            BindValue(txtBillOfLadingId, Dataset.BillOfLading.BillOfLadingIDColumn.ColumnName);
            BindValue(txtPackage, Dataset.BillOfLading.ShipmentPackageIDColumn.ColumnName);

            mediaWidget.Setup(new Utilities.MediaWidget.SetupArgs
            {
                MediaJunctionTable = Dataset.BillOfLadingMedia,
                MediaTable = Dataset.Media,
                MediaJunctionTableParentRowColumn = Dataset.BillOfLadingMedia.BillOfLadingIDColumn,
                AllowEditing = Editable,
                MediaLinkType = Documents.LinkType.BillOfLading,
                DocumentLinkTable = Dataset.BillOfLadingDocumentLink,
                ScannerSettingsType = ScannerSettingsType.Order
            });

            _panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);


            if (CurrentRecord is OrdersDataSet.BillOfLadingRow billOfLadingRow)
            {
                System.Collections.Generic.List<DataRow> mediaRows = billOfLadingRow.GetBillOfLadingMediaRows().ToList<DataRow>();
                mediaWidget.LoadMedia(mediaRows,
                    billOfLadingRow.GetBillOfLadingDocumentLinkRows().ToList<DataRow>(),
                    billOfLadingRow.BillOfLadingID);
            }
            else
            {
                mediaWidget.ClearMedia();
            }
        }

        #endregion
    }
}

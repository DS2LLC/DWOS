using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;
using DWOS.UI.Utilities;
using Infragistics.Documents.RichText;
using System.Data;
using System.IO;
using System.Text;

namespace DWOS.UI.Sales.Order
{
    public partial class BatchCocInformation : DataPanel
    {
        #region Fields

        private BatchCOCTableAdapter _taBatchCoc;

        /// <summary>
        /// Rich text WPF control that provides HTML import/export.
        /// </summary>
        /// <remarks>
        /// Infragistics does not provide an HTML editor for Windows Forms.
        /// </remarks>
        private readonly Infragistics.Controls.Editors.XamRichTextEditor cocEditor;

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey =>
            Dataset.BatchCOC.BatchCOCIDColumn.ColumnName;

        #endregion

        #region Methods

        public BatchCocInformation()
        {
            InitializeComponent();
            cocEditor = new Infragistics.Controls.Editors.XamRichTextEditor();
            cocElementHost.Child = cocEditor;
        }

        public void LoadData(OrdersDataSet dataset, BatchCOCTableAdapter taBatchCoc)
        {
            _dataset = dataset;
            _taBatchCoc = taBatchCoc;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.BatchCOC.TableName;

            BindValue(txtBatchCoc, Dataset.BatchCOC.BatchCOCIDColumn.ColumnName);
            BindValue(txtBatch, Dataset.BatchCOC.BatchIDColumn.ColumnName);
            BindValue(dteDateCertified, Dataset.BatchCOC.DateCertifiedColumn.ColumnName);
            BindValue(cboCreatedBy, Dataset.BatchCOC.QAUserColumn.ColumnName);

            BindList(cboCreatedBy, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            _panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);


            if (CurrentRecord is OrdersDataSet.BatchCOCRow cocRow)
            {
                //not allowed to edit saved versions
                cocEditor.IsReadOnly = cocRow != null && cocRow.BatchCOCID > 0;
                LoadCocInfo();
            }
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            SaveCocInfo();
            base.BeforeMoveToNewRecord(id);
        }

        public override void EndEditing()
        {
            //ensure saved
            SaveCocInfo();
            base.EndEditing();
        }

        private void LoadCocInfo()
        {
            if (!(CurrentRecord is OrdersDataSet.BatchCOCRow cocRow))
            {
                return;
            }

            //if null then not loaded
            if (cocRow.IsCOCInfoNull())
            {
                var rowState = cocRow.RowState;
                cocRow.COCInfo = _taBatchCoc.GetCocInfo(cocRow.BatchCOCID);

                //if row was unchanged then accept changes to prevent seeing this as an update
                if (rowState == DataRowState.Unchanged)
                {
                    cocRow.AcceptChanges();
                }
            }

            if (!cocRow.IsCOCInfoNull())
            {
                var htmlBody = cocRow.IsCompressed
                    ? cocRow.COCInfo.DecompressString()
                    : cocRow.COCInfo;

                var htmlDocument = HtmlUtilities.CreateHtmlDocument(htmlBody);
                using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlDocument)))
                {
                    cocEditor.Document.LoadFromHtml(memStream);
                }
            }
        }

        private void SaveCocInfo()
        {
            if (base.CurrentRecord is OrdersDataSet.BatchCOCRow cocRow && cocRow.IsValidState() && !cocEditor.IsReadOnly)
            {
                var originalValue = cocRow.IsCompressed
                    ? cocRow.COCInfo.DecompressString()
                    : cocRow.COCInfo;

                string newValue;
                using (var memStream = new MemoryStream())
                {
                    cocEditor.Document.SaveToHtml(memStream);
                    memStream.Flush();
                    memStream.Position = 0;

                    using (var reader = new StreamReader(memStream))
                    {
                        newValue = HtmlUtilities.ExtractHtmlBody(reader.ReadToEnd());
                    }
                }

                if (originalValue != newValue)
                {
                    cocRow.COCInfo = newValue.CompressString();
                    cocRow.IsCompressed = true;
                }
            }
        }

        #endregion
    }
}

using System.Data;
using System.Linq;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;

namespace DWOS.UI.Sales
{
    public partial class OrderPartMarking : DataPanel
    {
        #region Fields

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.OrderPartMark.OrderPartMarkIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public OrderPartMarking() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderPartMark.TableName;

            base.BindValue(this.txtDef1, Dataset.OrderPartMark.Line1Column.ColumnName);
            base.BindValue(this.txtDef2, Dataset.OrderPartMark.Line2Column.ColumnName);
            base.BindValue(this.txtDef3, Dataset.OrderPartMark.Line3Column.ColumnName);
            base.BindValue(this.txtDef4, Dataset.OrderPartMark.Line4Column.ColumnName);
            base.BindValue(this.txtSpecification, Dataset.OrderPartMark.ProcessSpecColumn.ColumnName);
            base.BindValue(this.txtDate, Dataset.OrderPartMark.PartMarkedDateColumn.ColumnName);

            base._panelLoaded = true;
        }

        public OrdersDataSet.OrderPartMarkRow Add(OrdersDataSet.OrderRow order)
        {
            var partId = order.PartID;
            var rowVw = bsData.AddNew() as DataRowView;
            var partMarkRow = rowVw.Row as OrdersDataSet.OrderPartMarkRow;

            partMarkRow.OrderRow = order;

            using (var dsParts = new PartsDataset() { EnforceConstraints = false })
            {
                using (var taPart = new Data.Datasets.PartsDatasetTableAdapters.PartTableAdapter())
                    taPart.FillByPartID(dsParts.Part, partId);

                var part = dsParts.Part.FirstOrDefault();

                if (part != null)
                {
                    using (var taPartMark = new Data.Datasets.PartsDatasetTableAdapters.Part_PartMarkingTableAdapter())
                    {
                        taPartMark.FillByPartID(dsParts.Part_PartMarking, partId);
                    }

                    if (part.GetPart_PartMarkingRows().Length > 0)
                    {
                        CopyFromPart(part, partMarkRow);
                    }
                    else if (!part.IsAirframeNull())
                    {
                        CopyFromAirframe(partId, partMarkRow);
                    }
                }
            }

            return partMarkRow;
        }

        private static void CopyFromPart(PartsDataset.PartRow part, OrdersDataSet.OrderPartMarkRow partMarkRow)
        {
            var markFromPart = part?.GetPart_PartMarkingRows()?.FirstOrDefault();

            if (markFromPart == null || partMarkRow == null)
            {
                return;
            }

            if (!markFromPart.IsProcessSpecNull())
            {
                partMarkRow.ProcessSpec = markFromPart.ProcessSpec;
            }

            if (!markFromPart.IsDef1Null())
            {
                partMarkRow.Line1 = markFromPart.Def1;
            }

            if (!markFromPart.IsDef2Null())
            {
                partMarkRow.Line2 = markFromPart.Def2;
            }

            if (!markFromPart.IsDef3Null())
            {
                partMarkRow.Line3 = markFromPart.Def3;
            }

            if (!markFromPart.IsDef4Null())
            {
                partMarkRow.Line4 = markFromPart.Def4;
            }
        }

        private static void CopyFromAirframe(int partId, OrdersDataSet.OrderPartMarkRow partMarkRow)
        {
            using (var taPM = new PartMarkingTableAdapter())
            {
                var pmDT = taPM.GetDataByPart(partId);

                var pmTemplate = pmDT.FirstOrDefault();

                if (pmTemplate != null)
                {
                    partMarkRow.PartMarkingID = pmTemplate.PartMarkingID;
                    partMarkRow.ProcessSpec = pmTemplate.ProcessSpec;

                    if (!pmTemplate.IsDef1Null())
                        partMarkRow.Line1 = pmTemplate.Def1;
                    if (!pmTemplate.IsDef2Null())
                        partMarkRow.Line2 = pmTemplate.Def2;
                    if (!pmTemplate.IsDef3Null())
                        partMarkRow.Line3 = pmTemplate.Def3;
                    if (!pmTemplate.IsDef4Null())
                        partMarkRow.Line4 = pmTemplate.Def4;
                }
            }
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var pm = CurrentRecord as OrdersDataSet.OrderPartMarkRow;

            //if part mark has already been applied then you cannot edit
            this.txtDef1.Enabled = pm != null && pm.IsPartMarkedDateNull();
            this.txtDef2.Enabled = pm != null && pm.IsPartMarkedDateNull();
            this.txtDef3.Enabled = pm != null && pm.IsPartMarkedDateNull();
            this.txtDef4.Enabled = pm != null && pm.IsPartMarkedDateNull();
        }

        #endregion
    }
}
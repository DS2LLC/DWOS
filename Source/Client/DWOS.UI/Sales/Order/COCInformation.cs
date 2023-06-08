using System;
using System.Data;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrdersDataSetTableAdapters;

namespace DWOS.UI.Sales
{
    public partial class COCInformation : DataPanel
    {
        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.COC.COCIDColumn.ColumnName; }
        }

        private COCTableAdapter CocTA { get; set; }

        #endregion

        #region Methods

        public COCInformation() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset, COCTableAdapter taCOC)
        {
            CocTA = taCOC;
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.COC.TableName;

            base.BindValue(this.dtDateCertified, Dataset.COC.DateCertifiedColumn.ColumnName);
            base.BindValue(this.cboCOCUserCreated, Dataset.COC.QAUserColumn.ColumnName);
            base.BindValue(this.numPartQty, Dataset.COC.PartQuantityColumn.ColumnName);
            base.BindValue(this.txtCOCID, Dataset.COC.COCIDColumn.ColumnName);
            //base.BindValue(this.txtCOCInfo, this.Dataset.COC.COCInfoColumn.ColumnName);

            base.BindList(this.cboCOCUserCreated, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);

            base._panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            var cocRow = base.CurrentRecord as OrdersDataSet.COCRow;

            if(cocRow != null && cocRow.COCID > 0)
            {
                //not allowed to edit saved versions
                this.numPartQty.ReadOnly = true;
                this.txtCOCInfo.ReadOnly = true;
            }
            else
            {
                this.numPartQty.ReadOnly = false;
                this.txtCOCInfo.ReadOnly = false;
            }

            LoadCOCInfo(cocRow);
        }

        protected override void BeforeMoveToNewRecord(object id)
        {
            SaveCOCInfo();
            base.BeforeMoveToNewRecord(id);
        }

        /// <summary>
        ///     Loads the coc information when a record is loaded as we dont need it until viewing.
        /// </summary>
        /// <param name="cocRow">The coc row.</param>
        private void LoadCOCInfo(OrdersDataSet.COCRow cocRow)
        {
            this.txtCOCInfo.Value = null;

            if(cocRow != null)
            {
                //if null then not loaded
                if(cocRow.IsCOCInfoNull())
                {
                    var rowState = cocRow.RowState;
                    cocRow.COCInfo = CocTA.GetCOCInfo(cocRow.COCID);

                    //if row was un modified then accept changes to prevent seeing this as an update
                    if(rowState == DataRowState.Unchanged)
                        cocRow.AcceptChanges();
                }

                if(!cocRow.IsCOCInfoNull())
                    this.txtCOCInfo.Value = cocRow.IsCompressed ? cocRow.COCInfo.DecompressString() : cocRow.COCInfo;
            }
        }

        private void SaveCOCInfo()
        {
            var cocRow = base.CurrentRecord as OrdersDataSet.COCRow;

            if(cocRow != null && cocRow.RowState != DataRowState.Deleted)
            {
                if(!this.txtCOCInfo.ReadOnly && this.txtCOCInfo.Value != null)
                {
                    var originalValue = cocRow.IsCompressed ? cocRow.COCInfo.DecompressString() : cocRow.COCInfo;
                    var newValue = this.txtCOCInfo.Value.ToString();

                    if(originalValue != newValue)
                    {
                        cocRow.COCInfo = this.txtCOCInfo.Value.ToString().CompressString();
                        cocRow.IsCompressed = true;
                    }
                }
            }
        }

        public override void EndEditing()
        {
            //ensure saved
            SaveCOCInfo();
            base.EndEditing();
        }

        public OrdersDataSet.COCRow AddCOCRow(OrdersDataSet.COCRow existingCOC, int userID)
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as OrdersDataSet.COCRow;
            cr.OrderID = existingCOC.OrderID;
            cr.COCInfo = existingCOC.COCInfo;
            cr.PartQuantity = existingCOC.PartQuantity;
            cr.QAUser = userID;
            cr.DateCertified = existingCOC.DateCertified;
            cr.IsCompressed = existingCOC.IsCompressed;

            return cr;
        }

        #endregion

        #region Events

        private void grpData_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtCOCInfo.Height = grpData.Height - this.txtCOCInfo.Top - 3;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error on resize.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }
}
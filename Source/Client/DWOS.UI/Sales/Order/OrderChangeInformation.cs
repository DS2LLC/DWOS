using System;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.ListsDataSetTableAdapters;

namespace DWOS.UI.Sales
{
    public partial class OrderChangeInformation : DataPanel
    {
        #region Fields

        public event Action <int> GoToOrder;

        #endregion

        #region Properties

        public OrdersDataSet Dataset
        {
            get { return base._dataset as OrdersDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.OrderChange.OrderChangeIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public OrderChangeInformation() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderChange.TableName;

            base.BindValue(this.dtDateCertified, Dataset.OrderChange.DateCreatedColumn.ColumnName);
            base.BindValue(this.txtOriginalOrder, Dataset.OrderChange.ParentOrderIDColumn.ColumnName);
            base.BindValue(this.txtReworkOrder, Dataset.OrderChange.ChildOrderIDColumn.ColumnName);
            base.BindValue(this.cboChangeType, Dataset.OrderChange.ChangeTypeColumn.ColumnName);
            base.BindValue(this.cboReason, Dataset.OrderChange.ReasonCodeColumn.ColumnName);
            base.BindValue(this.cboCOCUserCreated, Dataset.OrderChange.UserIDColumn.ColumnName);

            base.BindList(this.cboCOCUserCreated, Dataset.UserSummary, Dataset.UserSummary.UserIDColumn.ColumnName, Dataset.UserSummary.NameColumn.ColumnName);
            using(var ta = new d_OrderChangeTypeTableAdapter())
            {
                var changeTypes = ta.GetData();
                base.BindList(this.cboChangeType, changeTypes, changeTypes.OrderChangeTypeColumn.ColumnName, changeTypes.NameColumn.ColumnName);
            }

            using(var ta = new d_OrderChangeReasonTableAdapter())
            {
                var changeReasons = ta.GetData();
                base.BindList(this.cboReason, changeReasons, changeReasons.OrderChangeReasonIDColumn.ColumnName, changeReasons.NameColumn.ColumnName);
            }

            base._panelLoaded = true;
        }

        #endregion

        #region Events

        private void btnGoToOriginal_Click(object sender, EventArgs e)
        {
            if(GoToOrder != null)
            {
                var internalRework = CurrentRecord as OrdersDataSet.OrderChangeRow;
                if(internalRework != null)
                    GoToOrder(internalRework.ParentOrderID);
            }
        }

        private void btnGoToRework_Click(object sender, EventArgs e)
        {
            if(GoToOrder != null)
            {
                var internalRework = CurrentRecord as OrdersDataSet.OrderChangeRow;
                if(internalRework != null)
                    GoToOrder(internalRework.ChildOrderID);
            }
        }

        #endregion
    }
}
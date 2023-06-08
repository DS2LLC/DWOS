using System;
using DWOS.Data.Datasets;

namespace DWOS.UI.Sales.Order
{
    public partial class SerialNumberQuickViewPanel : DataPanel
    {
        public OrdersDataSet Dataset
        {
            get { return _dataset as OrdersDataSet; }
            set { _dataset = value; }
        }

        protected override string BindingSourcePrimaryKey => Dataset.Order.OrderIDColumn.ColumnName;

        public bool ViewOnly { get; internal set; }

        public SerialNumberQuickViewPanel()
        {
            InitializeComponent();
        }

        public void LoadData(OrdersDataSet dsOrders)
        {
            _dataset = dsOrders;
            bsData.DataSource = dsOrders;
            bsData.DataMember = dsOrders.Order.TableName;
            advSerialNumbers.LoadData(dsOrders);

            advSerialNumbers.ReadOnly = ViewOnly;
            _panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);
            advSerialNumbers.LoadOrder(Convert.ToInt32(id));
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Utilities.Validation;

namespace DWOS.UI.Sales
{
    public partial class OrderContainerInfo : DataPanel
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
            get { return Dataset.OrderContainers.OrderIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public OrderContainerInfo() { InitializeComponent(); }

        public void LoadData(OrdersDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.OrderContainers.TableName;

            orderContainerWidget.LoadData(dataset);

            base._panelLoaded = true;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            // Current Record is not an order row - lookup order in dataset
            OrdersDataSet.OrderRow currentOrder = null;
            if (id is int orderId)
            {
                currentOrder = Dataset.Order.FindByOrderID(orderId);
            }

            orderContainerWidget.LoadOrder(currentOrder);
        }

        public override void EndEditing()
        {
            base.EndEditing();
            orderContainerWidget.EndEditing();
        }

        public override void CancelEdits()
        {
            base.CancelEdits();
            orderContainerWidget.CancelEdits();
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider) { }

        public OrdersDataSet.OrderContainersRow[] AddContainers(int orderID, int containerCount, int partsPerContainer)
        {
            var containers = new List <OrdersDataSet.OrderContainersRow>();

            for(var i = 0; i < containerCount; i++)
            {
                var rowVw = bsData.AddNew() as DataRowView;

                var cr = rowVw.Row as OrdersDataSet.OrderContainersRow;
                cr.OrderID = orderID;
                cr.PartQuantity = partsPerContainer;
                cr.IsActive = true;
                cr.ShipmentPackageTypeRow = Dataset.ShipmentPackageType.OrderBy(type => type.ShipmentPackageTypeID).FirstOrDefault();
                cr.EndEdit();

                containers.Add(cr);
            }

            return containers.ToArray();
        }

        #endregion
    }
}